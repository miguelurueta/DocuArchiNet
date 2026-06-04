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
import { InteractionManagerPluginPackage } from "@embedpdf/plugin-interaction-manager/react";
import { SelectionPluginPackage } from "@embedpdf/plugin-selection/react";
import { AnnotationPluginPackage } from "@embedpdf/plugin-annotation";
import { SignatureMode, SignaturePluginPackage } from "@embedpdf/plugin-signature";

export function createBasicPluginRegistration() {
  return [
    createPluginRegistration(DocumentManagerPluginPackage),
    createPluginRegistration(ViewportPluginPackage),
    createPluginRegistration(ScrollPluginPackage),
    createPluginRegistration(RenderPluginPackage),
    // Dependencias oficiales para selección/interacción/annotations/signatures.
    // Importante: registrar como plugins oficiales (sin lógica custom).
    createPluginRegistration(InteractionManagerPluginPackage),
    createPluginRegistration(SelectionPluginPackage),
    createPluginRegistration(AnnotationPluginPackage, {
      // Enterprise: las firmas se deben poder eliminar, pero no mover/redimensionar.
      // Evita que aparezcan handles de resize/drag (data-epdf-handle) alrededor de la firma.
      tools: [
        {
          id: "signatureStamp",
          interaction: {
            exclusive: false,
            isDraggable: false,
            isResizable: false,
          },
        },
        {
          id: "signatureInk",
          interaction: {
            exclusive: false,
            isDraggable: false,
            isResizable: false,
          },
        },
      ],
    }),
    createPluginRegistration(SignaturePluginPackage, {
      // Enterprise default: solo firma (no iniciales) hasta que el producto requiera ambos.
      mode: SignatureMode.SignatureOnly,
    }),
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
