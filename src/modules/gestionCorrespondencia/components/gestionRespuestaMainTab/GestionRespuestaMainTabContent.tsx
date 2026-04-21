import { CarryOutFilled, MailFilled } from "@ant-design/icons";
import { useEffect, useId, useState } from "react";
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
const MOBILE_MEDIA_QUERY = "(max-width: 768px)";

const useMediaQuery = (query: string) => {
  const getMatches = () =>
    typeof window !== "undefined" ? window.matchMedia(query).matches : false;
  const [matches, setMatches] = useState(getMatches);

  useEffect(() => {
    const mediaQueryList = window.matchMedia(query);
    const update = (event: MediaQueryListEvent) => {
      setMatches(event.matches);
    };

    setMatches(mediaQueryList.matches);
    mediaQueryList.addEventListener("change", update);
    return () => {
      mediaQueryList.removeEventListener("change", update);
    };
  }, [query]);

  return matches;
};

type GestionRespuestaMainTabContentProps = {
  idTareaWf?: number;
};

export function GestionRespuestaMainTabContent(
  _props: GestionRespuestaMainTabContentProps = {},
) {
  const panelId = useId();
  const isCompact = useMediaQuery(DEFAULT_MEDIA_QUERY);
  const isMobile = useMediaQuery(MOBILE_MEDIA_QUERY);
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

  useEffect(() => {
    setIsPanelCollapsed(isCompact);
  }, [isCompact]);

  return (
    <section className={styles.mainTab} aria-label="Contenido principal de respuesta">
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
              onClick: () => setIsGestionDocumentoModalOpen(true),
            },
          ]}
        />

        <div
          className={styles.workbenchBody}
          data-panel-collapsed={isPanelCollapsed}
          data-variant={isMobile ? "overlay" : "inline"}
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
              paginationMode="visual"
              pageFormat="A4"
              pageOrientation="portrait"
              pageMargins={{ top: 96, right: 72, bottom: 96, left: 72 }}
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
        onClose={() => setIsGestionDocumentoModalOpen(false)}
      />
    </section>
  );
}
