import { expect, test } from "@playwright/test";

type AuthResponse = {
  data?: {
    token?: string;
    expiracion?: string;
    usuario?: {
      permisos?: string[];
    };
  };
};

function getRequiredEnv(name: string) {
  const env = (globalThis as any).process?.env;
  const value = env?.[name];
  if (!value || value.trim().length === 0) {
    throw new Error(`Missing required env var: ${name}`);
  }
  return value.trim();
}

async function loginByApi(request: any) {
  const apiUrl = ((globalThis as any).process?.env?.PLAYWRIGHT_API_URL ?? "http://localhost/DocuArchiApi").replace(/\/+$/, "");

  const response = await request.post(`${apiUrl}/api/accout/ValidaUserAplicacion`, {
    data: {
      IdEmpresa: Number(getRequiredEnv("PLAYWRIGHT_LOGIN_EMPRESA_ID")),
      IdModulo: Number(getRequiredEnv("PLAYWRIGHT_LOGIN_MODULO_ID")),
      User: getRequiredEnv("PLAYWRIGHT_LOGIN_USER"),
      Password: getRequiredEnv("PLAYWRIGHT_LOGIN_PASSWORD"),
    },
  });

  expect(response.ok()).toBeTruthy();

  const body = (await response.json()) as AuthResponse;

  return {
    token: body.data?.token as string,
    expiracion: body.data?.expiracion as string,
    permisos: body.data?.usuario?.permisos ?? [],
  };
}

test.describe("AppEditor PRO E2E + Performance REAL", () => {
  test("flujo completo + medicion avanzada", async ({ page, request }) => {

    // 🔐 LOGIN
    const session = await loginByApi(request);

    await page.addInitScript((auth) => {
      localStorage.setItem("token", auth.token);
      localStorage.setItem("token-expiracion", auth.expiracion);
      localStorage.setItem("permisos", JSON.stringify(auth.permisos));
    }, session);

    // ⏱ CARGA
    const startLoad = Date.now();
    await page.goto("/dashboard/gestion-correspondencia/respuesta/924");
    await page.waitForLoadState("networkidle");
    const loadTime = Date.now() - startLoad;

    console.log("⏱ Tiempo de carga:", loadTime, "ms");

    // 🎯 UI
    await expect(
      page.getByRole("region", { name: /contenido principal de respuesta/i })
    ).toBeVisible();

    const workbench = page.getByTestId("gestion-respuesta-workbench");
    const saveButton = workbench.getByRole("button", { name: "Guardar" });

    await expect(saveButton).toBeVisible();

    // ✍️ EDITOR
    const editor = page.locator('[contenteditable="true"]');
    await expect(editor).toBeVisible();

    // ⚡ 1. TIEMPO DE FOCUS (RENDER INTERNO)
    const focusStart = Date.now();
    await editor.click();
    const focusTime = Date.now() - focusStart;

    console.log("🎯 Tiempo focus editor:", focusTime, "ms");

    // ⚡ 2. INPUT LAG (PRUEBA REAL)
    const lagStart = Date.now();

    await page.keyboard.type("1234567890".repeat(30), { delay: 0 });

    const lagTime = Date.now() - lagStart;

    console.log("⌨️ Input lag editor:", lagTime, "ms");

    // ⚡ 3. ESCRITURA CONTROLADA
    const typingStart = Date.now();

    await editor.fill("Texto E2E de prueba rendimiento");

    const typingTime = Date.now() - typingStart;

    console.log("✍️ Tiempo escritura:", typingTime, "ms");

    await expect(editor).toContainText("Texto E2E de prueba rendimiento");

    // ⚡ 4. ESCRITURA MASIVA (PRUEBA DE ESTRÉS)
    const heavyText = "Performance Test ".repeat(200);

    const heavyStart = Date.now();

    await editor.fill(heavyText);

    const heavyTime = Date.now() - heavyStart;

    console.log("🔥 Escritura masiva:", heavyTime, "ms");

    // 💾 GUARDADO
    const startSave = Date.now();

    await saveButton.click();

    await page.waitForResponse((resp) =>
      resp.url().includes("/respuesta") && resp.status() === 200
    );

    const saveTime = Date.now() - startSave;

    console.log("💾 Tiempo guardado:", saveTime, "ms");

    // 📊 MÉTRICAS NAVEGADOR
    const metrics = await page.evaluate(() => {
      const nav = performance.getEntriesByType("navigation")[0] as PerformanceNavigationTiming;

      return {
        domContentLoaded: nav.domContentLoadedEventEnd,
        loadEvent: nav.loadEventEnd,
        firstPaint: performance.getEntriesByName("first-paint")[0]?.startTime,
        firstContentfulPaint: performance.getEntriesByName("first-contentful-paint")[0]?.startTime,
      };
    });

    console.log("📊 Browser metrics:", metrics);

    // 🚨 VALIDACIONES REALES
    expect(loadTime).toBeLessThan(5000);
    expect(focusTime).toBeLessThan(300);
    expect(lagTime).toBeLessThan(2000);
    expect(typingTime).toBeLessThan(800);
    expect(heavyTime).toBeLessThan(3000);
    expect(saveTime).toBeLessThan(2000);
  });
});