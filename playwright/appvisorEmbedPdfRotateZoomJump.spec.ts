import { expect, test } from "@playwright/test";

test("SCRUMCORE-206: zoom se deshabilita en rotate 90° (evitar jump)", async ({
  page,
}) => {
  await page.goto("/__playwright/embedpdf", { waitUntil: "domcontentloaded" });

  await expect(page.getByRole("toolbar", { name: "Toolbar PDF" })).toBeVisible({
    timeout: 20_000,
  });

  // wait first rendered page
  const firstImg = page.locator('img[src^="blob:"]').first();
  await expect(firstImg).toBeVisible({ timeout: 30_000 });

  // rotate right (90°)
  await page.getByRole("button", { name: "Rotar derecha" }).click();

  await expect(firstImg).toBeVisible({ timeout: 30_000 });

  const zoomIn = page.getByRole("button", { name: "Zoom in" });
  const zoomOut = page.getByRole("button", { name: "Zoom out" });
  const resetZoom = page.getByRole("button", { name: "Reset zoom" });

  await expect(zoomIn).toBeDisabled();
  await expect(zoomOut).toBeDisabled();
  await expect(resetZoom).toBeDisabled();
});
