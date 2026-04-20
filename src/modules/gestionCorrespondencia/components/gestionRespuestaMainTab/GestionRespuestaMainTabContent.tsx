import {
  CarryOutFilled,
  MailFilled,
} from "@ant-design/icons";
import { useId, useState, useSyncExternalStore } from "react";
import { AppCollapseRail } from "../../../../app/Components/UI/AppCollapseRail";
import {
  AppEditor,
  AppEditorSaveAction,
  useAppEditorSaveState,
} from "../../../../app/Components/UI/AppEditor";
import { AppToolbar } from "../../../../app/Components/UI/AppToolbar";
import type { AppUploadFile } from "../../../../app/Components/UI/AppUpload/AppUpload";
import { AppUpload } from "../../../../app/Components/UI/AppUpload/AppUpload";
import { useEstructuraRespuestaIdTarea } from "../../hooks/useEstructuraRespuestaIdTarea";
import styles from "./GestionRespuestaMainTabContent.module.css";
import { GestionRespuestaEditorContainer } from "./GestionRespuestaEditorContainer";
import { GestionRespuestaInfoHeader } from "./GestionRespuestaInfoHeader";
import { GestionDocumentoModal } from "./modalGestionDocumento";

const DEFAULT_MEDIA_QUERY = "(max-width: 1024px)";
const MOBILE_MEDIA_QUERY = "(max-width: 768px)";

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

export function GestionRespuestaMainTabContent({
  idTareaWf,
}: GestionRespuestaMainTabContentProps = {}) {
  const panelId = useId();
  const isCompact = useMediaQuery(DEFAULT_MEDIA_QUERY);
  const isMobile = useMediaQuery(MOBILE_MEDIA_QUERY);
  const [isPanelCollapsed, setIsPanelCollapsed] = useState(isCompact);
  const [isGestionDocumentoModalOpen, setIsGestionDocumentoModalOpen] = useState(false);
  const [files, setFiles] = useState<AppUploadFile[]>([]);
  const { estrucTuraRespuesta, loading, error, isEmpty } =
    useEstructuraRespuestaIdTarea(idTareaWf);

  const headerDescription =
    typeof idTareaWf !== "number"
      ? "No se pudo resolver el identificador de la tarea (idTareaWf)."
      : loading
        ? "Cargando estructura de respuesta..."
        : error
          ? `No fue posible cargar la estructura de respuesta: ${error.message}`
          : isEmpty
            ? `Sin datos de estructura para la tarea ${idTareaWf}.`
            : undefined;
  const [editorValue, setEditorValue] = useState<string>("");
  const [savedEditorValue, setSavedEditorValue] = useState<string>("");
  const { saveStatus } = useAppEditorSaveState({
    currentValue: editorValue,
    savedValue: savedEditorValue,
  });

  return (
    <section className={styles.mainTab} aria-label="Contenido principal de respuesta">
      <GestionRespuestaInfoHeader
        description={headerDescription}
        metadata={[
          { label: "Radicado", value: loading ? "..." : (estrucTuraRespuesta?.Radicado ?? "-") },
          { label: "Remitente", value: loading ? "..." : (estrucTuraRespuesta?.Destinatario ?? "-") },
          { label: "Trámite", value: estrucTuraRespuesta?.TramiteDocumento ?? "-" },
        ]}
      />

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
          <GestionRespuestaEditorContainer
            title="Editor principal"
            description="Zona dominante del workspace para construir la respuesta."
          >
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

          <AppCollapseRail
            title="Herramientas"
            collapsed={isPanelCollapsed}
            onToggle={() => setIsPanelCollapsed((prev) => !prev)}
            panelId={panelId}
            placement="right"
            variant={isMobile ? "overlay" : "inline"}
          >
            <div className={styles.toolsList}>
              <div className={styles.toolsItem}>
                <strong>Checklist de validacion</strong>
                <span className={styles.infoCopy}>
                  Estado del analisis y observaciones tecnicas.
                </span>
              </div>
              <div className={styles.toolsItem}>
                <strong>Referencias del expediente</strong>
                <span className={styles.infoCopy}>Links y notas operativas clave.</span>
              </div>
              <div className={styles.toolsItem}>
                <strong>Historial reciente</strong>
                <span className={styles.infoCopy}>
                  Resumen de cambios y actividades asociadas.
                </span>
              </div>
            </div>
          </AppCollapseRail>
        </div>
      </div>

      <div className={styles.attachments}>
        <div className={styles.attachmentsHeader}>
          <h3 className={styles.attachmentsTitle}>Adjuntos</h3>
          <span className={styles.infoCopy}>Carga de soportes y anexos del expediente.</span>
        </div>
        <AppUpload value={files} onChange={setFiles} drag size="md" />
      </div>

      <GestionDocumentoModal
        open={isGestionDocumentoModalOpen}
        onClose={() => setIsGestionDocumentoModalOpen(false)}
      />
    </section>
  );
}
