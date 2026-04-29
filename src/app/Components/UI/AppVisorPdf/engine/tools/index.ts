import type { AppVisorPdfTool } from "../../domain/visorPdf.types";
import { arrowTool } from "./arrowTool";
import { freehandTool } from "./freehandTool";
import { panTool } from "./panTool";
import { rectTool } from "./rectTool";
import { selectTool } from "./selectTool";
import { textTool } from "./textTool";
import type { FabricTool } from "./tool.types";

const tools: FabricTool[] = [
  panTool,
  selectTool,
  freehandTool,
  textTool,
  rectTool,
  arrowTool,
];

export function resolveTool(tool: AppVisorPdfTool): FabricTool {
  const resolved =
    tools.find((item) => item.tool === tool) ??
    tools.find((item) => item.tool === "select");
  if (!resolved) {
    throw new Error("No Fabric tool resolver available");
  }
  return resolved;
}

