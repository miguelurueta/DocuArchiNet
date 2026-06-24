import { expect, test } from "@playwright/test";

function isZoomTraceLine(text: string) {
  return (
    text.includes("[DV][zoom-trace]") ||
    text.includes("[DV][autofit]") ||
    text.includes("[DV][visor]")
  );
}

test("SCRUMCORE-267: auditoría runtime zoom +,+,-,- (AppVisorEmbedPdf)", async ({ page }) => {
  test.setTimeout(120_000);

  const traceLines: string[] = [];
  const toolbarSamples: string[] = [];

  page.on("console", async (message) => {
    const text = message.text();
    if (!isZoomTraceLine(text)) return;
    const args = await Promise.all(
      message.args().map(async (arg) => {
        try {
          return await arg.jsonValue();
        } catch {
          try {
            return await arg.toString();
          } catch {
            return "<arg-unserializable>";
          }
        }
      }),
    );

    const normalized = args
      .map((arg) => {
        if (typeof arg === "string") return arg;
        try {
          return JSON.stringify(arg);
        } catch {
          return String(arg);
        }
      })
      .join(" | ");

    traceLines.push(normalized);
  });

  page.on("pageerror", (error) => {
    traceLines.push(`[PAGE_ERROR] ${error.message}`);
  });

  await page.addInitScript(() => {
    (window as any).__DV_DEBUG__ = true;
  });

  await page.goto("/__playwright/embedpdf", { waitUntil: "domcontentloaded" });
  await expect(page.getByLabel("Zona de documento")).toBeVisible({ timeout: 20_000 });
  const toolbar = page.getByRole("toolbar", { name: "Toolbar PDF" });
  await expect(toolbar).toBeVisible({ timeout: 60_000 });

  const zoomLabel = page.getByLabel("Zoom actual");
  await expect(zoomLabel).toBeVisible();

  const snapshot = async (marker: string) => {
    const value = (await zoomLabel.textContent())?.trim() ?? "";
    toolbarSamples.push(`${marker}: ${value}`);
    return value;
  };

  const beforeOpen = await snapshot("open");

  await page.getByRole("button", { name: "Zoom in" }).click();
  await expect(zoomLabel).not.toHaveText(beforeOpen, { timeout: 10_000 });
  await page.waitForTimeout(200);
  const afterZoomIn1 = await snapshot("zoomIn#1");

  await page.getByRole("button", { name: "Zoom in" }).click();
  await expect(zoomLabel).not.toHaveText(afterZoomIn1, { timeout: 10_000 });
  await page.waitForTimeout(200);
  const afterZoomIn2 = await snapshot("zoomIn#2");

  await page.getByRole("button", { name: "Zoom out" }).click();
  await expect(zoomLabel).not.toHaveText(afterZoomIn2, { timeout: 10_000 });
  await page.waitForTimeout(200);
  const afterZoomOut1 = await snapshot("zoomOut#1");

  await page.getByRole("button", { name: "Zoom out" }).click();
  await expect(zoomLabel).not.toHaveText(afterZoomOut1, { timeout: 10_000 });
  await page.waitForTimeout(200);
  await snapshot("zoomOut#2");

  expect(toolbarSamples).toHaveLength(5);
  expect(traceLines.some((line) => line.includes("[DV][autofit]"))).toBeTruthy();
  expect(traceLines.some((line) => line.includes("[DV][zoom-trace][toolbar]") )).toBeTruthy();
  expect(
    traceLines.some((line) => line.includes('"action":"zoom-in"') || line.includes('action: "zoom-in"')),
  ).toBeTruthy();
  expect(
    traceLines.some((line) => line.includes('"action":"zoom-out"') || line.includes('action: "zoom-out"')),
  ).toBeTruthy();

  const grouped = [
    "--- TRAZA EJECUCIÓN ---",
    ...traceLines,
    "--- ZOOM (toolbar text sample) ---",
    ...toolbarSamples,
    "",
  ];
  console.info(grouped.join("\n"));
});



