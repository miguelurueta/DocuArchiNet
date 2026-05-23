import { expect, test } from "@playwright/test";
import type { APIRequestContext } from "@playwright/test";

function getRequiredEnv(name: string) {
  const value = process.env[name];
  if (!value || value.trim().length === 0) {
    throw new Error(`Missing required env var: ${name}`);
  }
  return value.trim();
}

async function loginByApi(request: APIRequestContext) {
  const apiUrl = (process.env.PLAYWRIGHT_API_URL ?? "http://localhost/DocuArchiApi").replace(
    /\/+$/,
    "",
  );

  const response = await request.post(`${apiUrl}/api/accout/ValidaUserAplicacion`, {
    data: {
      IdEmpresa: Number(getRequiredEnv("PLAYWRIGHT_LOGIN_EMPRESA_ID")),
      IdModulo: Number(getRequiredEnv("PLAYWRIGHT_LOGIN_MODULO_ID")),
      User: getRequiredEnv("PLAYWRIGHT_LOGIN_USER"),
      Password: getRequiredEnv("PLAYWRIGHT_LOGIN_PASSWORD"),
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
