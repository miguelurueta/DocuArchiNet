import { expect, test } from "@playwright/test";

test("SCRUMCORE-191: carga fixture PDF y renderiza pagina 1 a canvas", async ({
  page,
}) => {
  await page.goto("/", { waitUntil: "domcontentloaded" });
  await page.waitForLoadState("networkidle").catch(() => undefined);

  const run = async () =>
    page.evaluate(async () => {
    const buildMinimalPdfBytes = () => {
      const parts: string[] = [];
      const offsets: number[] = [];
      const push = (chunk: string) => {
        offsets.push(parts.join("").length);
        parts.push(chunk);
      };

      parts.push("%PDF-1.4\n");
      // 1: Catalog
      push("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");
      // 2: Pages
      push("2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n");
      // 3: Page
      push(
        "3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 200 200] /Contents 4 0 R /Resources << >> >>\nendobj\n",
      );
      // 4: Empty contents
      push("4 0 obj\n<< /Length 0 >>\nstream\n\nendstream\nendobj\n");

      const xrefStart = parts.join("").length;
      const xref: string[] = [];
      xref.push("xref\n0 5\n");
      xref.push("0000000000 65535 f \n");
      for (let i = 0; i < offsets.length; i += 1) {
        const off = String(offsets[i]).padStart(10, "0");
        xref.push(`${off} 00000 n \n`);
      }

      const trailer =
        `trailer\n<< /Size 5 /Root 1 0 R >>\nstartxref\n${xrefStart}\n%%EOF\n`;

      const pdf = parts.join("") + xref.join("") + trailer;
      const bytes = new Uint8Array(pdf.length);
      for (let i = 0; i < pdf.length; i += 1) {
        bytes[i] = pdf.charCodeAt(i) & 0xff;
      }
      return bytes;
    };

    const { createPdfjsEngine } = await import(
      "/src/app/Components/UI/AppVisorPdf/engine/pdfjsEngine.ts"
    );

    const engine = createPdfjsEngine({ maxCacheEntries: 12 });
    const bytes = buildMinimalPdfBytes();
    const load = await engine.load({ kind: "bytes", bytes });

    const canvas = document.createElement("canvas");
    document.body.appendChild(canvas);
    const render = await engine.renderPage({ pageNumber: 1, zoom: 1 }, canvas);

    engine.destroy();

    return {
      pageCount: load.pageCount,
      width: render.width,
      height: render.height,
    };
  });

  let result: { pageCount: number; width: number; height: number } | null = null;
  for (let attempt = 0; attempt < 3; attempt += 1) {
    try {
      result = await run();
      break;
    } catch (error) {
      const message = error instanceof Error ? error.message : String(error);
      if (!message.toLowerCase().includes("execution context was destroyed")) {
        throw error;
      }
      await page.waitForTimeout(500);
    }
  }
  if (!result) {
    throw new Error("No se pudo ejecutar el evaluate de engine (navegación recurrente).");
  }

  expect(result.pageCount).toBe(1);
  expect(result.width).toBeGreaterThan(0);
  expect(result.height).toBeGreaterThan(0);
});
