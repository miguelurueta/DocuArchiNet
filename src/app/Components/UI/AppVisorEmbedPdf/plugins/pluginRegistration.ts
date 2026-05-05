import { createPluginRegistration } from "../engine/embedPdfAdapter";
import { DocumentManagerPluginPackage } from "@embedpdf/plugin-document-manager";
import { ViewportPluginPackage } from "@embedpdf/plugin-viewport";
import { ScrollPluginPackage } from "@embedpdf/plugin-scroll";
import { RenderPluginPackage } from "@embedpdf/plugin-render";

export function createBasicPluginRegistration() {
  return [
    createPluginRegistration(DocumentManagerPluginPackage),
    createPluginRegistration(ViewportPluginPackage),
    createPluginRegistration(ScrollPluginPackage),
    createPluginRegistration(RenderPluginPackage),
  ];
}
