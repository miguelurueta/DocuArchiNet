import { expect, test } from "@playwright/test";

test("SCRUMCORE-204: toolbar zoom in/out/reset funciona (plugin oficial)", async ({
  page,
}) => {
  await page.goto("/__playwright/embedpdf", { waitUntil: "domcontentloaded" });

  const toolbar = page.getByRole("toolbar", { name: "Toolbar PDF" });
  await expect(toolbar).toBeVisible({ timeout: 20_000 });

  const zoomLabel = page.getByLabel("Zoom actual");
  await expect(zoomLabel).toBeVisible();

  const initial = (await zoomLabel.textContent())?.trim() ?? "";

  await page.getByRole("button", { name: "Zoom in" }).click();
  await expect(zoomLabel).not.toHaveText(initial, { timeout: 10_000 });

  const afterZoomIn = (await zoomLabel.textContent())?.trim() ?? "";

  await page.getByRole("button", { name: "Zoom out" }).click();
  await expect(zoomLabel).not.toHaveText(afterZoomIn, { timeout: 10_000 });

  await page.getByRole("button", { name: "Reset zoom" }).click();
  await expect(zoomLabel).toHaveText(/100%/, { timeout: 10_000 });
});

