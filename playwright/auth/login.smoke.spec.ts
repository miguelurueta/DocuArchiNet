import { expect, test } from "@playwright/test";

test.describe("Login Smoke", () => {
  test("renderiza el formulario principal de autenticacion", async ({ page }) => {
    await page.goto("/");

    await expect(page.getByRole("button", { name: /iniciar sesi[oó]n/i })).toBeVisible();
    await expect(page.locator("#usuario")).toBeVisible();
    await expect(page.locator("#password")).toBeVisible();
    await expect(page.getByRole("link", { name: /olvidaste/i })).toBeVisible();
  });

  test("permite diligenciar credenciales y enviar el formulario", async ({ page }) => {
    await page.goto("/");

    await page.locator("#usuario").fill("usuario.demo");
    await page.locator("#password").fill("123456");
    await page.getByRole("button", { name: /iniciar sesi[oó]n/i }).click();

    await expect(page.locator("#usuario")).toHaveValue("usuario.demo");
    await expect(page.locator("#password")).toHaveValue("123456");
  });
});
