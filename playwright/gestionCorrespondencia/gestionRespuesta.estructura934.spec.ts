import { expect, test } from "@playwright/test";
import type { APIRequestContext } from "@playwright/test";
import { mkdirSync } from "node:fs";

const ENDPOINT_PART = "/api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea";

type EstructuraResponseLike = {
  success?: unknown;
  Success?: unknown;
  data?: unknown;
  Data?: unknown;
};

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

test.describe("GestionCorrespondencia real - estructura 934", () => {
  test("captura payload y estado del bloqueo", async ({ page, request }) => {
    const session = await loginByApi(request);

    await page.addInitScript((auth) => {
      localStorage.setItem("token", auth.token);
      localStorage.setItem("token-expiracion", auth.expiracion);
      localStorage.setItem("permisos", JSON.stringify(auth.permisos));
    }, session);

    const calls: Array<{ status: number; body: unknown }> = [];

    page.on("response", async (resp) => {
      if (!resp.url().includes(ENDPOINT_PART)) return;
      try {
        const json = (await resp.json()) as unknown;
        calls.push({ status: resp.status(), body: json });
      } catch {
        calls.push({ status: resp.status(), body: "<non-json>" });
      }
    });

    await page.goto("/dashboard/gestion-correspondencia/respuesta/934");

    const detail = page.getByTestId("gestion-correspondencia-detail-region");
    await expect(detail).toBeVisible();

    const detailBody = detail.locator('[data-detail-state]');
    await expect(detailBody).toBeVisible();

    await page.waitForTimeout(2000);

    const attrs = await detailBody.evaluate((node) => {
      const el = node as HTMLElement;
      const get = (name: string) => el.getAttribute(name) ?? "";
      return {
        detailState: get("data-detail-state"),
        loading: get("data-estructura-loading"),
        resolved: get("data-estructura-resolved"),
        empty: get("data-estructura-empty"),
        emptyConfirmed: get("data-estructura-empty-confirmed"),
        error: get("data-estructura-error"),
      };
    });

    console.log("[E2E] detail attrs:", attrs);
    console.log("[E2E] endpoint calls:", calls.length);
    calls.forEach((c, idx) => console.log(`[E2E] call#${idx + 1} status=${c.status}`, c.body));

    const last = calls[calls.length - 1]?.body as EstructuraResponseLike | undefined;
    const rawData = last?.data ?? last?.Data;
    const items = Array.isArray(rawData) ? rawData.length : rawData ? 1 : 0;
    console.log("[E2E] last payload items:", items);

    mkdirSync("playwright-artifacts", { recursive: true });
    await page.screenshot({ path: "playwright-artifacts/gestionRespuesta-934.png", fullPage: true });
  });
});
