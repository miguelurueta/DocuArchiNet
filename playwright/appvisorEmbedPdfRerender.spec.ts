import { expect, test } from "@playwright/test";

test("SCRUMCORE-203: AppVisorEmbedPdf re-render estable (sin warnings de hooks)", async ({
  page,
}) => {
  const consoleMessages: string[] = [];
  page.on("console", (msg) => {
    const text = msg.text();
    if (!text) return;
    consoleMessages.push(text);
  });

  await page.goto("/__playwright/embedpdf", { waitUntil: "domcontentloaded" });

  // Espera a que el visor exista (el componente expone aria-label para el contenedor del documento).
  await expect(page.getByLabel("Zona de documento")).toBeVisible({ timeout: 20_000 });

  // Fuerza un re-render visible: resize + navegación ida/vuelta
  await page.setViewportSize({ width: 900, height: 700 });
  await page.setViewportSize({ width: 1100, height: 800 });
  await page.reload({ waitUntil: "domcontentloaded" });
  await expect(page.getByLabel("Zona de documento")).toBeVisible({ timeout: 20_000 });

  const joined = consoleMessages.join("\n").toLowerCase();
  expect(joined).not.toContain("change in the order of hooks");
  expect(joined).not.toContain("rules of hooks");
  expect(joined).not.toContain("invalid hook call");
});

