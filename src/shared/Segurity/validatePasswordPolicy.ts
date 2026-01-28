export function validatePasswordPolicy(password: string) {
  const errors: string[] = [];

  if (password.length < 8)
    errors.push("Mínimo 8 caracteres");

  if (!/[A-Z]/.test(password))
    errors.push("Debe incluir una mayúscula");

  if (!/[a-z]/.test(password))
    errors.push("Debe incluir una minúscula");

  if (!/[0-9]/.test(password))
    errors.push("Debe incluir un número");

  if (!/[!@#$%^&*()_+\-=[\]{};':"\\|,.<>/?]/.test(password))
    errors.push("Debe incluir un carácter especial");

  if (/\s/.test(password))
    errors.push("No debe contener espacios");

  return {
    valid: errors.length === 0,
    message: errors.join(" · "),
  };
}
