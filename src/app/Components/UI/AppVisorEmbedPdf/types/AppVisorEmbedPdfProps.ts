import type React from "react";
import type { AppVisorEmbedPdfRef, AppVisorLoadInput, AppVisorLoadResult, ViewerEffectivePermissions } from "../AppVisorEmbedPdf.types";

export interface AppVisorEmbedPdfProps {
  fileUrl?: string;
  /**
   * Solo UX: cuando true, el visor puede mostrar un estado de carga (p.ej. skeleton en toolbar)
   * y bloquear interacciones mientras se obtiene la fuente (fileUrl/blobUrl).
   */
  loading?: boolean;
  className?: string;
  style?: React.CSSProperties;
  onEmptyDocumentHintRequest?: () => void;
  onSaveAnnotatedPages?: () => void;
  isSaveAnnotatedPagesDisabled?: boolean;
  isSavingAnnotatedPages?: boolean;
  saveAnnotatedPagesProgress?: number;
}

export type { AppVisorEmbedPdfRef, AppVisorLoadInput, AppVisorLoadResult, ViewerEffectivePermissions };

