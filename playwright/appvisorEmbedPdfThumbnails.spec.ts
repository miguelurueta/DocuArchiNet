import { expect, test } from "@playwright/test";

test("SCRUMCORE-205: toggle thumbnails abre/cierra sin warnings de hooks", async ({
  page,
}) => {
  const consoleMessages: string[] = [];
  page.on("console", (msg) => {
    const text = msg.text();
    if (!text) return;
    consoleMessages.push(text);
  });

  await page.goto("/__playwright/embedpdf", { waitUntil: "domcontentloaded" });

  const toggle = page.getByRole("button", { name: "Abrir thumbnails" });
  await expect(toggle).toBeVisible({ timeout: 20_000 });

  await expect(page.getByLabel("Panel thumbnails")).toHaveCount(0);

  await toggle.click();
  await expect(page.getByLabel("Panel thumbnails")).toBeVisible();
  await expect(toggle).toHaveAttribute("aria-pressed", "true");

  await toggle.click();
  await expect(page.getByLabel("Panel thumbnails")).toHaveCount(0);
  await expect(toggle).toHaveAttribute("aria-pressed", "false");

  const joined = consoleMessages.join("\n").toLowerCase();
  expect(joined).not.toContain("change in the order of hooks");
  expect(joined).not.toContain("rules of hooks");
  expect(joined).not.toContain("invalid hook call");
});
