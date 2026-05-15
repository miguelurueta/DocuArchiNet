import { memo, useEffect, useMemo, useRef, useState } from "react";
import { createPortal } from "react-dom";
import { CloseOutlined, UploadOutlined } from "@ant-design/icons";
import {
  SignatureDrawPad,
  useSignatureUpload,
} from "@embedpdf/plugin-signature/react";
import type {
  SignatureFieldDefinition,
  SignatureInkFieldDefinition,
  SignatureStampFieldDefinition,
} from "@embedpdf/plugin-signature";

import styles from "./AppPdfSignatureModal.module.css";

type TabKey = "draw" | "upload";

export interface AppPdfSignatureModalProps {
  isOpen: boolean;
  onClose(): void;
  onStartPlacement(signature: SignatureFieldDefinition): void;
  isPlacementReady?: boolean;
}

export const AppPdfSignatureModal = memo(function AppPdfSignatureModal({
  isOpen,
  onClose,
  onStartPlacement,
  isPlacementReady = true,
}: AppPdfSignatureModalProps) {
  const [tab, setTab] = useState<TabKey>("draw");
  const [current, setCurrent] = useState<SignatureFieldDefinition | null>(null);
  const [uploadedFileName, setUploadedFileName] = useState<string | null>(null);
  const [drawResetKey, setDrawResetKey] = useState(0);

  const initialFocusRef = useRef<HTMLButtonElement | null>(null);

  useEffect(() => {
    if (!isOpen) return;
    initialFocusRef.current?.focus();
  }, [isOpen]);

  useEffect(() => {
    if (!isOpen) return;
    // Reset UI state on open so a previous filename doesn't leak across sessions.
    setUploadedFileName(null);
  }, [isOpen]);

  useEffect(() => {
    if (!isOpen) return;
    const onKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    window.addEventListener("keydown", onKeyDown);
    return () => window.removeEventListener("keydown", onKeyDown);
  }, [isOpen, onClose]);

  const accept = useMemo(() => "image/png,image/jpeg,image/svg+xml", []);
  const upload = useSignatureUpload({
    accept,
    onResult: (result) => {
      setCurrent(result as SignatureStampFieldDefinition | null);
    },
  });

  if (!isOpen) return null;

  const canUse = Boolean(current) && isPlacementReady;
  const clearUploadedSignature = () => {
    setCurrent(null);
    setUploadedFileName(null);
    if (upload.inputRef.current) upload.inputRef.current.value = "";
  };
  const resetModalStateAfterUse = () => {
    setCurrent(null);
    setUploadedFileName(null);
    if (upload.inputRef.current) upload.inputRef.current.value = "";
    // Si venimos de "Draw", forzar reset del canvas para no dejar el trazo anterior.
    setDrawResetKey((v) => v + 1);
  };

  const content = (
    <div
      className={styles.overlay}
      role="dialog"
      aria-label="Firmas"
      aria-modal="true"
      onMouseDown={(e) => {
        if (e.target === e.currentTarget) onClose();
      }}
    >
      <div className={styles.panel}>
        <div className={styles.header}>
          <h3 className={styles.title}>Firmas</h3>
          <button
            ref={initialFocusRef}
            type="button"
            className={styles.close}
            onClick={onClose}
            aria-label="Cerrar modal de firmas"
            title="Cerrar"
          >
            <CloseOutlined />
          </button>
        </div>

        <div className={styles.body}>
          <p className={styles.hint}>
            Selecciona una firma y luego haz click sobre el PDF para ubicarla
            (placement oficial EmbedPDF).
            {!isPlacementReady ? " (Inicializando plugins\u2026)" : null}
          </p>

          <div className={styles.tabs} role="tablist" aria-label="Tipo de firma">
            <button
              type="button"
              className={`${styles.tab} ${tab === "draw" ? styles.tabActive : ""}`}
              onClick={() => {
                setTab("draw");
                setCurrent(null);
              }}
              aria-label="Dibujar firma"
              aria-pressed={tab === "draw"}
            >
              Dibujar firma
            </button>
            <button
              type="button"
              className={`${styles.tab} ${tab === "upload" ? styles.tabActive : ""}`}
              onClick={() => {
                setTab("upload");
                setCurrent(null);
                setUploadedFileName(null);
              }}
              aria-label="Subir imagen"
              aria-pressed={tab === "upload"}
            >
              Subir firma
            </button>
          </div>

          <div className={styles.padShell}>
            {tab === "draw" ? (
              <SignatureDrawPad
                key={drawResetKey}
                onResult={(result: SignatureInkFieldDefinition | null) =>
                  setCurrent(result)
                }
              />
            ) : null}

            {tab === "upload" ? (
              <>
                <input
                  ref={upload.inputRef}
                  type="file"
                  accept={upload.accept}
                  style={{ display: "none" }}
                  onChange={(e) => {
                    const file = e.currentTarget.files?.[0] ?? null;
                    setUploadedFileName(file?.name ?? null);
                    upload.handleFileInputChange(e.nativeEvent);
                  }}
                />
                <button
                  type="button"
                  className={styles.primary}
                  onClick={() => upload.openFilePicker()}
                  aria-label="Subir imagen de firma"
                  title={uploadedFileName ? "Reemplazar firma" : "Subir firma"}
                >
                  <UploadOutlined aria-hidden="true" /> {uploadedFileName ? "Reemplazar firma" : "Subir firma"}
                </button>
                {uploadedFileName ? (
                  <div className={styles.uploadMeta} aria-label="Archivo adjunto">
                    <span className={styles.uploadMetaName} title={uploadedFileName}>
                      {uploadedFileName}
                    </span>
                    <button
                      type="button"
                      className={styles.uploadMetaClear}
                      onClick={clearUploadedSignature}
                      aria-label="Quitar firma adjunta"
                      title="Quitar firma adjunta"
                    >
                      <CloseOutlined aria-hidden="true" />
                    </button>
                  </div>
                ) : null}
              </>
            ) : null}
          </div>

          <div className={styles.actions}>
            {tab === "draw" && current ? (
              <button
                type="button"
                className={styles.secondary}
                onClick={() => {
                  setCurrent(null);
                  setDrawResetKey((v) => v + 1);
                }}
                aria-label="Limpiar firma"
                title="Limpiar firma"
              >
                Limpiar
              </button>
            ) : null}
            <button
              type="button"
              className={styles.primary}
              onClick={() => {
                if (!current) return;
                onStartPlacement(current);
                resetModalStateAfterUse();
              }}
              aria-label="Usar firma"
              title="Usar firma"
              disabled={!canUse}
            >
              Usar firma
            </button>
          </div>
        </div>
      </div>
    </div>
  );

  // Portal a `document.body` para evitar stacking contexts del Workbench (overflow/transform),
  // y garantizar que el modal cubra toda la UI (incluido Navbar sticky).
  return typeof document !== "undefined" ? createPortal(content, document.body) : content;
});
