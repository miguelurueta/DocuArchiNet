import { memo, useEffect, useMemo, useRef, useState } from "react";
import { CloseOutlined, UploadOutlined } from "@ant-design/icons";
import {
  SignatureDrawPad,
  SignatureTypePad,
  useSignatureUpload,
} from "@embedpdf/plugin-signature/react";
import type {
  SignatureFieldDefinition,
  SignatureInkFieldDefinition,
  SignatureStampFieldDefinition,
} from "@embedpdf/plugin-signature";

import styles from "./AppPdfSignatureModal.module.css";

type TabKey = "draw" | "type" | "upload";

export interface AppPdfSignatureModalProps {
  isOpen: boolean;
  onClose(): void;
  onStartPlacement(signature: SignatureFieldDefinition): void;
}

export const AppPdfSignatureModal = memo(function AppPdfSignatureModal({
  isOpen,
  onClose,
  onStartPlacement,
}: AppPdfSignatureModalProps) {
  const [tab, setTab] = useState<TabKey>("draw");
  const [current, setCurrent] = useState<SignatureFieldDefinition | null>(null);

  const initialFocusRef = useRef<HTMLButtonElement | null>(null);

  useEffect(() => {
    if (!isOpen) return;
    initialFocusRef.current?.focus();
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

  const canUse = Boolean(current);

  return (
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
              Draw
            </button>
            <button
              type="button"
              className={`${styles.tab} ${tab === "type" ? styles.tabActive : ""}`}
              onClick={() => {
                setTab("type");
                setCurrent(null);
              }}
              aria-label="Firma tipeada"
              aria-pressed={tab === "type"}
            >
              Type
            </button>
            <button
              type="button"
              className={`${styles.tab} ${tab === "upload" ? styles.tabActive : ""}`}
              onClick={() => {
                setTab("upload");
                setCurrent(null);
              }}
              aria-label="Subir imagen"
              aria-pressed={tab === "upload"}
            >
              Upload
            </button>
          </div>

          <div className={styles.padShell}>
            {tab === "draw" ? (
              <SignatureDrawPad
                onResult={(result: SignatureInkFieldDefinition | null) =>
                  setCurrent(result)
                }
              />
            ) : null}

            {tab === "type" ? (
              <SignatureTypePad
                onResult={(result: SignatureStampFieldDefinition | null) =>
                  setCurrent(result)
                }
                placeholder="Tu nombre"
              />
            ) : null}

            {tab === "upload" ? (
              <>
                <input
                  ref={upload.inputRef}
                  type="file"
                  accept={upload.accept}
                  style={{ display: "none" }}
                  onChange={(e) => upload.handleFileInputChange(e.nativeEvent)}
                />
                <button
                  type="button"
                  className={styles.primary}
                  onClick={() => upload.openFilePicker()}
                  aria-label="Subir imagen de firma"
                  title="Subir imagen"
                >
                  <UploadOutlined aria-hidden="true" /> Subir
                </button>
              </>
            ) : null}
          </div>

          <div className={styles.actions}>
            <button
              type="button"
              className={styles.primary}
              onClick={() => {
                if (!current) return;
                onStartPlacement(current);
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
});

