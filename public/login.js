(() => {
  const form = document.getElementById("login-form");
  if (!form) return;

  const emailField = form.querySelector('[data-field="email"]');
  const passwordField = form.querySelector('[data-field="password"]');
  const emailInput = document.getElementById("email");
  const passwordInput = document.getElementById("password");
  const emailError = document.getElementById("email-error");
  const passwordError = document.getElementById("password-error");
  const submit = document.getElementById("submit");
  const status = document.getElementById("status");
  const toggle = document.getElementById("toggle-password");
  const remember = document.getElementById("remember");

  // Password visibility toggle
  toggle.addEventListener("click", () => {
    const pressed = toggle.getAttribute("aria-pressed") === "true";
    toggle.setAttribute("aria-pressed", String(!pressed));
    passwordInput.type = pressed ? "password" : "text";
    toggle.setAttribute(
      "aria-label",
      pressed ? "Show password" : "Hide password"
    );
  });

  // Clear errors on input
  [emailInput, passwordInput].forEach((el) => {
    el.addEventListener("input", () => {
      const field = el.closest(".field");
      field.classList.remove("is-invalid");
      const errEl = field.querySelector(".field__error");
      if (errEl) errEl.textContent = "";
      status.textContent = "";
      status.classList.remove("is-error", "is-success");
    });
  });

  function setError(field, errEl, message) {
    field.classList.add("is-invalid");
    errEl.textContent = message;
  }

  function validate() {
    let ok = true;
    emailField.classList.remove("is-invalid");
    passwordField.classList.remove("is-invalid");
    emailError.textContent = "";
    passwordError.textContent = "";

    const email = emailInput.value.trim();
    const password = passwordInput.value;

    if (!email) {
      setError(emailField, emailError, "Enter your institutional email.");
      ok = false;
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      setError(emailField, emailError, "That email doesn't look right.");
      ok = false;
    }

    if (!password) {
      setError(passwordField, passwordError, "Enter your password.");
      ok = false;
    } else if (password.length < 6) {
      setError(passwordField, passwordError, "Passwords are at least 6 characters.");
      ok = false;
    }

    if (!ok) {
      form.classList.remove("is-invalid");
      // retrigger animation
      void form.offsetWidth;
      form.classList.add("is-invalid");
    }
    return ok;
  }

  async function onSubmit(event) {
    event.preventDefault();
    if (!validate()) return;

    const endpoint = form.dataset.endpoint || "/api/auth/login";
    const payload = {
      Email: emailInput.value.trim(),
      Password: passwordInput.value,
    };

    submit.classList.add("is-loading");
    submit.disabled = true;
    status.classList.remove("is-error", "is-success");
    status.textContent = "Contacting the scheduling desk…";

    try {
      const response = await fetch(endpoint, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },
        body: JSON.stringify(payload),
        credentials: "same-origin",
      });

      if (response.status === 401) {
        throw new Error("Invalid email or password.");
      }
      if (!response.ok) {
        const text = await response.text().catch(() => "");
        throw new Error(text || `Sign in failed (${response.status}).`);
      }

      const data = await response.json().catch(() => ({}));
      if (data && data.token) {
        const store = remember.checked ? localStorage : sessionStorage;
        try {
          store.setItem("auth.token", data.token);
          if (data.expiry) store.setItem("auth.expiry", data.expiry);
          if (data.fullName) store.setItem("auth.fullName", data.fullName);
        } catch (_) {
          /* storage unavailable; proceed anyway */
        }
      }

      status.classList.add("is-success");
      status.textContent = `Signed in${data && data.fullName ? ` as ${data.fullName}` : ""}. Redirecting…`;

      setTimeout(() => {
        const next = new URLSearchParams(location.search).get("next");
        location.assign(next && next.startsWith("/") ? next : "/dashboard");
      }, 700);
    } catch (err) {
      status.classList.add("is-error");
      status.textContent = err && err.message ? err.message : "Unable to sign in.";
      submit.classList.remove("is-loading");
      submit.disabled = false;
      passwordInput.select();
    }
  }

  form.addEventListener("submit", onSubmit);
})();
