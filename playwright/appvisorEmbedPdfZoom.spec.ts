import { expect, test } from "@playwright/test";

test("SCRUMCORE-204: toolbar zoom in/out/reset funciona (plugin oficial)", async ({
  page,
}) => {
  test.setTimeout(90_000);
  const pageErrors: string[] = [];
  page.on("pageerror", (err) => pageErrors.push(String(err)));
  page.on("console", (msg) => {
    if (msg.type() === "error") pageErrors.push(msg.text());
  });
  await page.goto("/__playwright/embedpdf", { waitUntil: "domcontentloaded" });
  await expect(page.getByLabel("Zona de documento")).toBeVisible({ timeout: 20_000 });

  const toolbar = page.getByRole("toolbar", { name: "Toolbar PDF" });
  try {
    await expect(toolbar).toBeVisible({ timeout: 60_000 });
  } catch (err) {
    const combined = pageErrors.filter(Boolean).join("\n");
    throw new Error(
      `Toolbar no visible. pageErrors:\n${combined || "(none)"}\nOriginal:\n${
        err instanceof Error ? err.message : String(err)
      }`,
    );
  }

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

  expect(pageErrors.join("\n").toLowerCase()).not.toContain("ocurrió un error inesperado");
});

