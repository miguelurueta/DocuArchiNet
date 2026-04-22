# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: gestionCorrespondencia\gestionRespuesta.estructura934.spec.ts >> GestionCorrespondencia real - estructura 934 >> captura payload y estado del bloqueo
- Location: playwright\gestionCorrespondencia\gestionRespuesta.estructura934.spec.ts:45:3

# Error details

```
Error: Missing required env var: PLAYWRIGHT_LOGIN_EMPRESA_ID
```

# Test source

```ts
  1   | import { expect, test } from "@playwright/test";
  2   | import type { APIRequestContext } from "@playwright/test";
  3   | import { mkdirSync } from "node:fs";
  4   | 
  5   | const ENDPOINT_PART = "/api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea";
  6   | 
  7   | type EstructuraResponseLike = {
  8   |   success?: unknown;
  9   |   Success?: unknown;
  10  |   data?: unknown;
  11  |   Data?: unknown;
  12  | };
  13  | 
  14  | function getRequiredEnv(name: string) {
  15  |   const value = process.env[name];
  16  |   if (!value || value.trim().length === 0) {
> 17  |     throw new Error(`Missing required env var: ${name}`);
      |           ^ Error: Missing required env var: PLAYWRIGHT_LOGIN_EMPRESA_ID
  18  |   }
  19  |   return value.trim();
  20  | }
  21  | 
  22  | async function loginByApi(request: APIRequestContext) {
  23  |   const apiUrl = (process.env.PLAYWRIGHT_API_URL ?? "http://localhost/DocuArchiApi").replace(/\/+$/, "");
  24  | 
  25  |   const response = await request.post(`${apiUrl}/api/accout/ValidaUserAplicacion`, {
  26  |     data: {
  27  |       IdEmpresa: Number(getRequiredEnv("PLAYWRIGHT_LOGIN_EMPRESA_ID")),
  28  |       IdModulo: Number(getRequiredEnv("PLAYWRIGHT_LOGIN_MODULO_ID")),
  29  |       User: getRequiredEnv("PLAYWRIGHT_LOGIN_USER"),
  30  |       Password: getRequiredEnv("PLAYWRIGHT_LOGIN_PASSWORD"),
  31  |     },
  32  |   });
  33  | 
  34  |   expect(response.ok()).toBeTruthy();
  35  |   const body = (await response.json()) as any;
  36  | 
  37  |   return {
  38  |     token: body.data?.token as string,
  39  |     expiracion: body.data?.expiracion as string,
  40  |     permisos: body.data?.usuario?.permisos ?? [],
  41  |   };
  42  | }
  43  | 
  44  | test.describe("GestionCorrespondencia real - estructura 934", () => {
  45  |   test("captura payload y estado del bloqueo", async ({ page, request }) => {
  46  |     const session = await loginByApi(request);
  47  | 
  48  |     await page.addInitScript((auth) => {
  49  |       localStorage.setItem("token", auth.token);
  50  |       localStorage.setItem("token-expiracion", auth.expiracion);
  51  |       localStorage.setItem("permisos", JSON.stringify(auth.permisos));
  52  |     }, session);
  53  | 
  54  |     const calls: Array<{ status: number; body: unknown }> = [];
  55  | 
  56  |     page.on("response", async (resp) => {
  57  |       if (!resp.url().includes(ENDPOINT_PART)) return;
  58  |       try {
  59  |         const json = (await resp.json()) as unknown;
  60  |         calls.push({ status: resp.status(), body: json });
  61  |       } catch {
  62  |         calls.push({ status: resp.status(), body: "<non-json>" });
  63  |       }
  64  |     });
  65  | 
  66  |     await page.goto("/dashboard/gestion-correspondencia/respuesta/934");
  67  | 
  68  |     const detail = page.getByTestId("gestion-correspondencia-detail-region");
  69  |     await expect(detail).toBeVisible();
  70  | 
  71  |     const detailBody = detail.locator('[data-detail-state]');
  72  |     await expect(detailBody).toBeVisible();
  73  | 
  74  |     await page.waitForTimeout(2000);
  75  | 
  76  |     const attrs = await detailBody.evaluate((node) => {
  77  |       const el = node as HTMLElement;
  78  |       const get = (name: string) => el.getAttribute(name) ?? "";
  79  |       return {
  80  |         detailState: get("data-detail-state"),
  81  |         loading: get("data-estructura-loading"),
  82  |         resolved: get("data-estructura-resolved"),
  83  |         empty: get("data-estructura-empty"),
  84  |         emptyConfirmed: get("data-estructura-empty-confirmed"),
  85  |         error: get("data-estructura-error"),
  86  |       };
  87  |     });
  88  | 
  89  |     console.log("[E2E] detail attrs:", attrs);
  90  |     console.log("[E2E] endpoint calls:", calls.length);
  91  |     calls.forEach((c, idx) => console.log(`[E2E] call#${idx + 1} status=${c.status}`, c.body));
  92  | 
  93  |     const last = calls[calls.length - 1]?.body as EstructuraResponseLike | undefined;
  94  |     const rawData = last?.data ?? last?.Data;
  95  |     const items = Array.isArray(rawData) ? rawData.length : rawData ? 1 : 0;
  96  |     console.log("[E2E] last payload items:", items);
  97  | 
  98  |     mkdirSync("playwright-artifacts", { recursive: true });
  99  |     await page.screenshot({ path: "playwright-artifacts/gestionRespuesta-934.png", fullPage: true });
  100 |   });
  101 | });
  102 | 
```