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
    data?: {
      token?: string;
      expiracion?: string;
      usuario?: { permisos?: unknown[] };
    };
  };

  return {
    token: body.data?.token ?? "",
    expiracion: body.data?.expiracion ?? "",
    permisos: body.data?.usuario?.permisos ?? [],
  };
}

test.describe("SCRUMCORE-229 - DocumentosWorkbench visual (real env)", () => {
  test("headers visibles tienen tooltip y look de título", async ({ page, request }) => {
    await page.setViewportSize({ width: 1366, height: 768 });
    const session = await loginByApi(request);

    const consoleErrors: string[] = [];
    page.on("console", (msg) => {
      if (msg.type() === "error") consoleErrors.push(msg.text());
    });

    await page.addInitScript((auth) => {
      localStorage.setItem("token", auth.token);
      localStorage.setItem("token-expiracion", auth.expiracion);
      localStorage.setItem("permisos", JSON.stringify(auth.permisos));
    }, session);

    await page.goto("/dashboard/gestion-correspondencia/respuesta/934");

    const workbench = page.getByTestId("documentos-workbench");
    await expect(workbench).toBeVisible();

    const documentoHeader = workbench.getByRole("columnheader", { name: "Documento" });
    await expect(documentoHeader).toBeVisible();
    await expect(documentoHeader).toHaveAttribute("title", /Documento/i);

    const accionesHeader = workbench.getByRole("columnheader", { name: /acciones/i });
    await expect(accionesHeader).toBeVisible();
    await expect(accionesHeader).toHaveAttribute("title", /acciones/i);

    const headerCell = workbench.locator(".ag-header-cell").first();
    const headerCellBorderRight = await headerCell.evaluate((node) => {
      const style = window.getComputedStyle(node as HTMLElement);
      return style.borderRightWidth;
    });
    expect(headerCellBorderRight).toBe("0px");

    expect(consoleErrors).toEqual([]);
  });

  test("click en fila selecciona la fila completa (aria-selected)", async ({ page, request }) => {
    await page.setViewportSize({ width: 1366, height: 768 });
    const session = await loginByApi(request);

    await page.addInitScript((auth) => {
      localStorage.setItem("token", auth.token);
      localStorage.setItem("token-expiracion", auth.expiracion);
      localStorage.setItem("permisos", JSON.stringify(auth.permisos));
    }, session);

    await page.goto("/dashboard/gestion-correspondencia/respuesta/934");

    const workbench = page.getByTestId("documentos-workbench");
    await expect(workbench).toBeVisible();

    const firstRow = workbench.locator(".ag-center-cols-container .ag-row").first();
    await expect(firstRow).toBeVisible();

    await firstRow.click();
    await expect(firstRow).toHaveAttribute("aria-selected", "true");

    await firstRow.hover();
    const hovered = workbench.locator(".ag-row-hover").first();
    await expect(hovered).toBeVisible();
  });

  test("focus visible en celda navegable (teclado)", async ({ page, request }) => {
    await page.setViewportSize({ width: 1366, height: 768 });
    const session = await loginByApi(request);

    await page.addInitScript((auth) => {
      localStorage.setItem("token", auth.token);
      localStorage.setItem("token-expiracion", auth.expiracion);
      localStorage.setItem("permisos", JSON.stringify(auth.permisos));
    }, session);

    await page.goto("/dashboard/gestion-correspondencia/respuesta/934");

    const workbench = page.getByTestId("documentos-workbench");
    await expect(workbench).toBeVisible();

    const firstCell = workbench.locator(".ag-center-cols-container .ag-row:first-child .ag-cell").nth(1);
    await firstCell.click();
    await page.keyboard.press("ArrowDown");

    const focusedCell = workbench.locator(".ag-cell-focus");
    await expect(focusedCell).toBeVisible();
  });
});
