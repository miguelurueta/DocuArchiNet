import { SaveOutlined, SendOutlined, ToolOutlined, UserAddOutlined } from "@ant-design/icons";
import { useEffect, useId, useState } from "react";
import { AppCollapseRail } from "../../../../app/Components/UI/AppCollapseRail";
import { AppToolbar } from "../../../../app/Components/UI/AppToolbar";
import type { AppUploadFile } from "../../../../app/Components/UI/AppUpload/AppUpload";
import { AppUpload } from "../../../../app/Components/UI/AppUpload/AppUpload";
import styles from "./GestionRespuestaMainTabContent.module.css";
import { GestionRespuestaEditorContainer } from "./GestionRespuestaEditorContainer";
import { GestionRespuestaInfoHeader } from "./GestionRespuestaInfoHeader";

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

export function GestionRespuestaMainTabContent() {
  const panelId = useId();
  const isCompact = useMediaQuery(DEFAULT_MEDIA_QUERY);
  const isMobile = useMediaQuery(MOBILE_MEDIA_QUERY);
  const [isPanelCollapsed, setIsPanelCollapsed] = useState(isCompact);
  const [files, setFiles] = useState<AppUploadFile[]>([]);

  useEffect(() => {
    setIsPanelCollapsed(isCompact);
  }, [isCompact]);

  return (
    <section className={styles.mainTab} aria-label="Contenido principal de respuesta">
      <GestionRespuestaInfoHeader
        metadata={[
          { label: "Origen", value: "Bandeja de correspondencia" },
          { label: "Estado", value: "Pendiente de validacion" },
          { label: "SLA", value: "4 horas restantes" },
        ]}
      />

      <div className={styles.workbench}>
        <AppToolbar
          className={styles.toolbar}
          actions={[
            { key: "guardar", label: "Guardar borrador", size: "sm", icon: <SaveOutlined /> },
            { key: "asignar", label: "Asignar revisor", size: "sm", icon: <UserAddOutlined /> },
          ]}
          primaryAction={{
            key: "enviar",
            label: "Enviar respuesta",
            size: "sm",
            icon: <SendOutlined />,
          }}
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
          />
          <AppCollapseRail
            title="Herramientas"
            collapsed={isPanelCollapsed}
            onToggle={() => setIsPanelCollapsed((prev) => !prev)}
            placement="right"
            variant={isMobile ? "overlay" : "inline"}
            panelId={panelId}
            railLabel="Herramientas"
            railIcon={<ToolOutlined />}
          >
            <div className={styles.toolsPanelSurface}>
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
    </section>
  );
}
