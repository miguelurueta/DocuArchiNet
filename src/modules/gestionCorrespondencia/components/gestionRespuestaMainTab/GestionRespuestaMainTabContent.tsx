import { CarryOutFilled, MailFilled } from "@ant-design/icons";
import { useCallback, useEffect, useId, useRef, useState, useSyncExternalStore } from "react";
import {
  AppEditor,
  AppEditorSaveAction,
  useAppEditorSaveState,
} from "../../../../app/Components/UI/AppEditor";
import { AppToolbar } from "../../../../app/Components/UI/AppToolbar";
import type { AppUploadFile } from "../../../../app/Components/UI/AppUpload/AppUpload";
import { AppUpload } from "../../../../app/Components/UI/AppUpload/AppUpload";
import styles from "./GestionRespuestaMainTabContent.module.css";
import { GestionRespuestaEditorContainer } from "./GestionRespuestaEditorContainer";
import { GestionRespuestaRightToolsPanel } from "./GestionRespuestaRightToolsPanel";
import { GestionDocumentoModal } from "./modalGestionDocumento";

const DEFAULT_MEDIA_QUERY = "(max-width: 1024px)";

const useMediaQuery = (query: string) => {
  const getSnapshot = () =>
    typeof window !== "undefined" ? window.matchMedia(query).matches : false;

  const subscribe = (onStoreChange: () => void) => {
    if (typeof window === "undefined") {
      return () => undefined;
    }

    const mediaQueryList = window.matchMedia(query);
    const onChange = () => onStoreChange();

    mediaQueryList.addEventListener("change", onChange);
    return () => {
      mediaQueryList.removeEventListener("change", onChange);
    };
  };

  return useSyncExternalStore(subscribe, getSnapshot, () => false);
};

type GestionRespuestaMainTabContentProps = {
  idTareaWf?: number;
};

export function GestionRespuestaMainTabContent(
  _props: GestionRespuestaMainTabContentProps = {},
) {
  const panelId = useId();
  const rootRef = useRef<HTMLElement | null>(null);
  const isCompact = useMediaQuery(DEFAULT_MEDIA_QUERY);
  const [isPanelCollapsed, setIsPanelCollapsed] = useState(isCompact);
  const [isGestionDocumentoModalOpen, setIsGestionDocumentoModalOpen] =
    useState(false);
  const [files, setFiles] = useState<AppUploadFile[]>([]);
  const [editorValue, setEditorValue] = useState<string>("");
  const [savedEditorValue, setSavedEditorValue] = useState<string>("");
  const { saveStatus } = useAppEditorSaveState({
    currentValue: editorValue,
    savedValue: savedEditorValue,
  });
  const canAdvanceToSend = files.length > 0;
  const goToSendStep = useCallback(() => {
    if (!canAdvanceToSend) {
      return;
    }
    setIsGestionDocumentoModalOpen(true);
  }, [canAdvanceToSend]);

  useEffect(() => {
    const root = rootRef.current;
    if (!root || typeof MutationObserver === "undefined") return;

    const isHidden = () => {
      const hiddenAttr = root.hasAttribute("hidden");
      const ariaHidden = root.getAttribute("aria-hidden") === "true";
      return hiddenAttr || ariaHidden;
    };

    const observer = new MutationObserver(() => {
      if (isHidden()) {
        setIsPanelCollapsed(true);
      }
    });

    observer.observe(root, {
      attributes: true,
      attributeFilter: ["hidden", "aria-hidden", "style", "class"],
    });

    return () => observer.disconnect();
  }, []);

  return (
    <section
      ref={(node) => {
        rootRef.current = node;
      }}
      className={styles.mainTab}
      aria-label="Contenido principal de respuesta"
    >
      <div className={styles.workbench}>
        <AppToolbar
          className={styles.toolbar}
          actions={[
            {
              key: "solicitud-aprobacion",
              label: "Solicitud de Aprobacion",
              size: "sm",
              variant: "ghost",
              icon: <CarryOutFilled />,
            },
            {
              key: "enviar",
              label: "Enviar",
              size: "sm",
              variant: "ghost",
              icon: <MailFilled />,
              onClick: () => goToSendStep(),
            },
          ]}
        />

        <div
          className={styles.workbenchBody}
          data-panel-collapsed={isPanelCollapsed}
          data-variant={isCompact ? "overlay" : "inline"}
          data-testid="gestion-respuesta-workbench"
        >
          <GestionRespuestaEditorContainer>
            <AppEditor
              value={editorValue}
              onChange={setEditorValue}
              toolbarActions={
                <AppEditorSaveAction
                  iconOnly
                  saveStatus={saveStatus}
                  onSave={() => {
                    setSavedEditorValue(editorValue);
                  }}
                />
              }
              placeholder="Escribe aqui la respuesta principal..."
              aria-label="Contenido del editor principal de respuesta"
              className={styles.embeddedAppEditor}
              surfaceClassName={styles.embeddedAppEditorSurface}
              minHeight="100%"
            />
          </GestionRespuestaEditorContainer>
          <GestionRespuestaRightToolsPanel
            collapsed={isPanelCollapsed}
            panelId={panelId}
            onToggle={() => setIsPanelCollapsed((prev) => !prev)}
          />
        </div>
      </div>

      <div className={styles.attachments}>
        <div className={styles.attachmentsHeader}>
          <h3 className={styles.attachmentsTitle}>Adjuntos</h3>
          <span className={styles.infoCopy}>Carga de soportes y anexos del expediente.</span>
        </div>
        <AppUpload value={files} onChange={setFiles} drag size="sm" />
      </div>

      <GestionDocumentoModal
        open={isGestionDocumentoModalOpen}
        onClose={() => {
          setIsGestionDocumentoModalOpen(false);
        }}
      />
    </section>
  );
}
