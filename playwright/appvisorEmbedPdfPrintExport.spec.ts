import { expect, test } from "@playwright/test";

test("SCRUMCORE-207: print/export no crashea y export dispara download", async ({
  page,
}) => {
  const consoleMessages: string[] = [];
  const pageErrors: string[] = [];

  page.on("console", (msg) => {
    const text = msg.text();
    if (text) consoleMessages.push(text);
  });

  page.on("pageerror", (err) => {
    pageErrors.push(String(err));
  });

  await page.goto("/__playwright/embedpdf", { waitUntil: "domcontentloaded" });

  const printButton = page.getByRole("button", { name: "Print" });
  const exportButton = page.getByRole("button", { name: "Export" });

  await expect(printButton).toBeVisible({ timeout: 20_000 });
  await expect(exportButton).toBeVisible();

  await printButton.click();

  const downloadPromise = page.waitForEvent("download", { timeout: 20_000 });
  await exportButton.click();
  const download = await downloadPromise;
  await download.cancel();

  expect(pageErrors.join("\n").toLowerCase()).not.toContain("error");
  expect(consoleMessages.join("\n").toLowerCase()).not.toContain("error");
});

