import type React from "react";
import type { AppVisorEmbedPdfRef, AppVisorLoadInput, AppVisorLoadResult, ViewerEffectivePermissions } from "../AppVisorEmbedPdf.types";

export interface AppVisorEmbedPdfProps {
  fileUrl?: string;
  className?: string;
  style?: React.CSSProperties;
}

export type { AppVisorEmbedPdfRef, AppVisorLoadInput, AppVisorLoadResult, ViewerEffectivePermissions };

