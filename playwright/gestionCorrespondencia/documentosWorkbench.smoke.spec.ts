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
  const apiUrl = (process.env.PLAYWRIGHT_API_URL ?? "http://localhost/DocuArchiApi").replace(/\/+$/, "");

  const response = await request.post(`${apiUrl}/api/accout/ValidaUserAplicacion`, {
    data: {
      IdEmpresa: Number(getRequiredEnv("PLAYWRIGHT_LOGIN_EMPRESA_ID")),
      IdModulo: Number(getRequiredEnv("PLAYWRIGHT_LOGIN_MODULO_ID")),
      User: getRequiredEnv("PLAYWRIGHT_LOGIN_USER"),
      Password: getRequiredEnv("PLAYWRIGHT_LOGIN_PASSWORD"),
    },
  });

  expect(response.ok()).toBeTruthy();
  const body = (await response.json()) as any;

  return {
    token: body.data?.token as string,
    expiracion: body.data?.expiracion as string,
    permisos: body.data?.usuario?.permisos ?? [],
  };
}

test.describe("GestionCorrespondencia real - DocumentosWorkbench", () => {
  test("renderiza panel documentos (visor + rail) sin romper layout", async ({ page, request }) => {
    const session = await loginByApi(request);

    await page.addInitScript((auth) => {
      localStorage.setItem("token", auth.token);
      localStorage.setItem("token-expiracion", auth.expiracion);
      localStorage.setItem("permisos", JSON.stringify(auth.permisos));
    }, session);

    await page.goto("/dashboard/gestion-correspondencia/respuesta/934");

    const workbench = page.getByTestId("documentos-workbench");
    await expect(workbench).toBeVisible();

    await expect(page.getByTestId("documentos-workbench")).toHaveAttribute("data-variant", /inline|overlay/);
    await expect(page.getByRole("status", { name: "Zona de documento" })).toBeVisible();
    await expect(page.getByRole("button", { name: /Ocultar Visualizar documentos|Mostrar Visualizar documentos/i })).toBeVisible();
  });
});
