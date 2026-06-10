import { memo, useEffect, useMemo, useRef, useState } from "react";
import { createPortal } from "react-dom";
import { CloseOutlined, ReloadOutlined, UploadOutlined } from "@ant-design/icons";
import {
  SignatureDrawPad,
  useSignatureUpload,
} from "@embedpdf/plugin-signature/react";
import type {
  SignatureFieldDefinition,
  SignatureInkFieldDefinition,
  SignatureStampFieldDefinition,
} from "@embedpdf/plugin-signature";
import { SignatureCreationType } from "@embedpdf/plugin-signature";

import styles from "./AppPdfSignatureModal.module.css";
import { useWorkflowPersonalSignature } from "../hooks/useWorkflowPersonalSignature";

type TabKey = "draw" | "upload" | "personal";

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

  const personal = useWorkflowPersonalSignature();

  const initialFocusRef = useRef<HTMLButtonElement | null>(null);

  useEffect(() => {
    if (!isOpen) return;
    initialFocusRef.current?.focus();
  }, [isOpen]);

  useEffect(() => {
    if (!isOpen) return;
    // Reset UI state on open so a previous filename doesn't leak across sessions.
    setUploadedFileName(null);
    // UX enterprise: abrir siempre en un estado consistente (evita quedar "pegado" en Firma personal
    // con ObjectURL revocado o estado idle).
    setTab("draw");
    setCurrent(null);
    personal.clear();
  }, [isOpen]);

  useEffect(() => {
    if (isOpen) return;
    // Garantiza cleanup (ObjectURL) y no persistir firma temporal fuera del modal.
    personal.clear();
  }, [isOpen, personal]);

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
    personal.clear();
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
                personal.clear();
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
                personal.clear();
              }}
              aria-label="Subir imagen"
              aria-pressed={tab === "upload"}
            >
              Subir firma
            </button>
            <button
              type="button"
              className={`${styles.tab} ${tab === "personal" ? styles.tabActive : ""}`}
              onClick={() => {
                setTab("personal");
                setCurrent(null);
                setUploadedFileName(null);
                // Carga just-in-time: al entrar a la pestaña.
                void personal.load();
              }}
              aria-label="Firma personal"
              aria-pressed={tab === "personal"}
            >
              Firma personal
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

            {tab === "personal" ? (
              <div aria-label="Firma personal">
                {personal.status === "loading" ? (
                  <div className={styles.hint} aria-label="Cargando firma personal">
                    Cargando firma personal\u2026
                  </div>
                ) : null}

                {personal.status === "empty" ? (
                  <div className={styles.hint} aria-label="Sin firma personal configurada">
                    No hay firma personal configurada para este usuario.
                  </div>
                ) : null}

                {personal.status === "error" ? (
                  <div className={styles.hint} aria-label="Error cargando firma personal">
                    {personal.errorMessage ?? "No fue posible cargar la firma personal."}
                    <div className={styles.actions}>
                      <button
                        type="button"
                        className={styles.secondary}
                        onClick={() => {
                          personal.clear();
                          void personal.load();
                        }}
                        aria-label="Reintentar carga de firma personal"
                        title="Reintentar"
                      >
                        <ReloadOutlined aria-hidden="true" /> Reintentar
                      </button>
                    </div>
                  </div>
                ) : null}

                {personal.status === "ready" && personal.blobUrl && personal.imageData ? (
                  <>
                    <div className={styles.previewCard} aria-label="Vista previa firma personal">
                      <img
                        className={styles.previewImg}
                        src={personal.blobUrl}
                        alt="Firma personal"
                        draggable={false}
                        onError={() => {
                          // Fallback simple: si falla el preview, mantenemos el CTA habilitado,
                          // pero mostramos un mensaje de negocio (sin exponer URLs).
                          // No cambiamos estado del hook para evitar loops.
                          // eslint-disable-next-line no-console
                          console.warn("[Firma personal] No fue posible previsualizar la imagen.");
                        }}
                      />
                      <div className={styles.previewMeta} aria-label="Metadata firma personal">
                        {personal.meta?.fileName ? (
                          <div className={styles.previewFileName} title={personal.meta.fileName}>
                            {personal.meta.fileName}
                          </div>
                        ) : (
                          <div className={styles.previewFileName}>Firma personal</div>
                        )}
                      </div>
                    </div>
                  </>
                ) : null}
              </div>
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
                if (tab === "personal") {
                  if (!personal.blobUrl || !personal.imageData) return;
                  const stamp: SignatureStampFieldDefinition = {
                    creationType: SignatureCreationType.Upload,
                    previewDataUrl: personal.blobUrl,
                    imageMimeType: personal.meta?.contentType,
                    imageData: personal.imageData,
                  };
                  onStartPlacement(stamp);
                  resetModalStateAfterUse();
                  return;
                }

                if (!current) return;
                onStartPlacement(current);
                resetModalStateAfterUse();
              }}
              aria-label="Usar firma"
              title="Usar firma"
              disabled={
                tab === "personal"
                  ? !isPlacementReady || !personal.blobUrl || !personal.imageData
                  : !canUse
              }
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
