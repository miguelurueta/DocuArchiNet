import { expect, test } from "@playwright/test";

test("SCRUMCORE-206: rotate izquierda/derecha sin warnings de hooks", async ({
  page,
}) => {
  const consoleMessages: string[] = [];
  page.on("console", (msg) => {
    const text = msg.text();
    if (!text) return;
    consoleMessages.push(text);
  });

  await page.goto("/__playwright/embedpdf", { waitUntil: "domcontentloaded" });

  const rotateLeft = page.getByRole("button", { name: "Rotar izquierda" });
  const rotateRight = page.getByRole("button", { name: "Rotar derecha" });

  await expect(rotateLeft).toBeVisible({ timeout: 20_000 });
  await expect(rotateRight).toBeVisible();

  await rotateRight.click();
  await rotateLeft.click();

  const joined = consoleMessages.join("\n").toLowerCase();
  expect(joined).not.toContain("change in the order of hooks");
  expect(joined).not.toContain("rules of hooks");
  expect(joined).not.toContain("invalid hook call");
});

