import { LeftOutlined, RightOutlined } from "@ant-design/icons";
import { Typography } from "antd";
import { AppButton } from "../../../../app/Components/UI/AppButton";
import styles from "./GestionRespuestaMainTabContent.module.css";

export type GestionRespuestaRightToolsPanelProps = {
  collapsed: boolean;
  panelId: string;
  onToggle: () => void;
};

export function GestionRespuestaRightToolsPanel({
  collapsed,
  panelId,
  onToggle,
}: GestionRespuestaRightToolsPanelProps) {
  const toggleLabel = collapsed
    ? "Mostrar panel de herramientas"
    : "Ocultar panel de herramientas";

  return (
    <>
      <aside
        className={styles.toolsPanel}
        data-collapsed={collapsed}
        aria-label="Panel de herramientas"
      >
        <div className={styles.toolsPanelHeader}>
          <Typography.Title level={5} className={styles.toolsPanelTitle}>
            Herramientas
          </Typography.Title>
          <AppButton
            variant="ghost"
            size="sm"
            onClick={onToggle}
            aria-controls={panelId}
            aria-expanded={!collapsed}
            aria-label={toggleLabel}
            className={styles.toolsToggle}
            icon={<RightOutlined />}
          >
          </AppButton>
        </div>
        <div id={panelId} className={styles.toolsPanelSurface}>
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
      </aside>

      {collapsed ? (
        <div className={styles.toolsRail} data-collapsed={collapsed}>
          <AppButton
            variant="secondary"
            size="sm"
            onClick={onToggle}
            aria-controls={panelId}
            aria-expanded={!collapsed}
            aria-label={toggleLabel}
            className={styles.toolsRestore}
            icon={<LeftOutlined />}
          />
        </div>
      ) : null}
    </>
  );
}
