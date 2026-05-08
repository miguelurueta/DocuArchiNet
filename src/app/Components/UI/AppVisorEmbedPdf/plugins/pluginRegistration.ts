import { createPluginRegistration } from "../engine/embedPdfAdapter";
import { DocumentManagerPluginPackage } from "@embedpdf/plugin-document-manager";
import { ViewportPluginPackage } from "@embedpdf/plugin-viewport";
import { ScrollPluginPackage } from "@embedpdf/plugin-scroll";
import { RenderPluginPackage } from "@embedpdf/plugin-render";
import { ZoomPluginPackage } from "@embedpdf/plugin-zoom";
import { ThumbnailPluginPackage } from "@embedpdf/plugin-thumbnail";
import { RotatePluginPackage } from "@embedpdf/plugin-rotate";
import { PrintPluginPackage } from "@embedpdf/plugin-print/react";
import { ExportPluginPackage } from "@embedpdf/plugin-export/react";

export function createBasicPluginRegistration() {
  return [
    createPluginRegistration(DocumentManagerPluginPackage),
    createPluginRegistration(ViewportPluginPackage),
    createPluginRegistration(ScrollPluginPackage),
    createPluginRegistration(RenderPluginPackage),
    createPluginRegistration(ZoomPluginPackage, {
      // Guardrail enterprise: evita zooms extremos que pueden colgar el browser (memoria/tiempo).
      maxZoom: 4,
      zoomStep: 0.1,
    }),
    createPluginRegistration(ThumbnailPluginPackage, {
      autoScroll: true,
      scrollBehavior: "smooth",
    }),
    createPluginRegistration(RotatePluginPackage),
    createPluginRegistration(PrintPluginPackage),
    createPluginRegistration(ExportPluginPackage),
  ];
}
