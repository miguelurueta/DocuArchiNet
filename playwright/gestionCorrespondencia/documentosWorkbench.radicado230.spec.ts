import { expect, test } from "@playwright/test";
import type { APIRequestContext } from "@playwright/test";

function getOptionalEnv(name: string) {
  const value = process.env[name];
  return value?.trim() ?? "";
}

function ensureEnvOrSkip() {
  const required = [
    "PLAYWRIGHT_LOGIN_EMPRESA_ID",
    "PLAYWRIGHT_LOGIN_MODULO_ID",
    "PLAYWRIGHT_LOGIN_USER",
    "PLAYWRIGHT_LOGIN_PASSWORD",
  ];

  const missing = required.filter((name) => getOptionalEnv(name).length === 0);
  test.skip(
    missing.length > 0,
    `E2E requiere variables de entorno. Faltan: ${missing.join(", ")}`,
  );
}

async function loginByApi(request: APIRequestContext) {
  const apiUrl = (process.env.PLAYWRIGHT_API_URL ?? "http://localhost/DocuArchiApi").replace(
    /\/+$/,
    "",
  );

  const empresaId = getOptionalEnv("PLAYWRIGHT_LOGIN_EMPRESA_ID");
  const moduloId = getOptionalEnv("PLAYWRIGHT_LOGIN_MODULO_ID");
  const user = getOptionalEnv("PLAYWRIGHT_LOGIN_USER");
  const password = getOptionalEnv("PLAYWRIGHT_LOGIN_PASSWORD");

  const response = await request.post(`${apiUrl}/api/accout/ValidaUserAplicacion`, {
    data: {
      IdEmpresa: Number(empresaId),
      IdModulo: Number(moduloId),
      User: user,
      Password: password,
    },
  });

  expect(response.ok()).toBeTruthy();
  const body = (await response.json()) as {
    data?: { token?: string; expiracion?: string; usuario?: { permisos?: unknown[] } };
  };

  return {
    token: body.data?.token ?? "",
    expiracion: body.data?.expiracion ?? "",
    permisos: body.data?.usuario?.permisos ?? [],
  };
}

test.describe("SCRUMCORE-230 - DocumentosWorkbench Radicado filter (real env)", () => {
  test("cambiar entre tareas no deja listado stale (smoke)", async ({ page, request }) => {
    ensureEnvOrSkip();
    const session = await loginByApi(request);

    await page.addInitScript((auth) => {
      localStorage.setItem("token", auth.token);
      localStorage.setItem("token-expiracion", auth.expiracion);
      localStorage.setItem("permisos", JSON.stringify(auth.permisos));
    }, session);

    await page.goto("/dashboard/gestion-correspondencia/respuesta/934");

    const workbench = page.getByTestId("documentos-workbench");
    await expect(workbench).toBeVisible();

    // Basic assertion: table renders and does not crash when navigating to another task.
    await expect(workbench.locator(".ag-root")).toBeVisible();

    await page.goto("/dashboard/gestion-correspondencia/respuesta/935");
    await expect(workbench).toBeVisible();
    await expect(workbench.locator(".ag-root")).toBeVisible();
  });
});
