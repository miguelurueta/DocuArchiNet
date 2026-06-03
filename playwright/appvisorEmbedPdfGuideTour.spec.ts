import { expect, test } from "@playwright/test";

const viewports = [
  { name: "desktop", width: 1280, height: 900 },
  { name: "tablet", width: 900, height: 700 },
  { name: "mobile", width: 390, height: 760 },
] as const;

test("SCRUMCORE-235: guia interactiva abre, navega y cierra en viewports clave", async ({ page }) => {
  test.setTimeout(90_000);

  for (const viewport of viewports) {
    await page.setViewportSize({ width: viewport.width, height: viewport.height });
    await page.goto("/__playwright/embedpdf", { waitUntil: "domcontentloaded" });

    const toolbar = page.getByRole("toolbar", { name: "Toolbar PDF" });
    await expect(toolbar, viewport.name).toBeVisible({ timeout: 20_000 });

    const helpButton = page.getByRole("button", { name: "Guia interactiva" });
    await expect(helpButton, viewport.name).toBeVisible();
    await expect(helpButton, viewport.name).toHaveAttribute("title", /Ayuda/i);
    await expect(helpButton, viewport.name).toHaveAttribute("data-guide-tour-id", "pdf-help");

    await helpButton.click();

    const popover = page.locator(".driver-popover");
    await expect(popover, viewport.name).toBeVisible();
    await expect(popover, viewport.name).toContainText("Toolbar PDF");

    await popover.getByRole("button", { name: /^siguiente$/i }).click();
    await expect(popover, viewport.name).toContainText("Miniaturas");

    await popover.getByRole("button", { name: /^anterior$/i }).click();
    await expect(popover, viewport.name).toContainText("Toolbar PDF");

    await page.keyboard.press("Escape");
    await expect(popover, viewport.name).toHaveCount(0);

    await helpButton.click();
    await expect(popover, viewport.name).toBeVisible();

    for (let step = 0; step < 20; step += 1) {
      const next = popover.getByRole("button", { name: /^siguiente$/i });
      if ((await next.count()) === 0 || !(await next.first().isVisible())) break;
      await next.click();
    }

    await popover.getByRole("button", { name: /^finalizar$/i }).click();
    await expect(popover, viewport.name).toHaveCount(0);
  }
});
