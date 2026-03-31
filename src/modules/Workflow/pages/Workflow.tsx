import { FileSearchOutlined, SnippetsFilled, UnorderedListOutlined, UsergroupAddOutlined } from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { AppButton } from "../../../app/Components/UI/AppButton";
import { AppContent } from "../../../app/Components/UI/AppContent";
import { AppDropdown } from "../../../app/Components/UI/AppDropdown";
import { AppToolbar } from "../../../app/Components/UI/AppToolbar";
import styles from "../style/Workflow.module.css";

const dropdownItems = [
  {
    key: "recuperar-tarea",
    label: "Recuperar Tarea",
    leftIcon: <FileSearchOutlined />,
  },
  {
    key: "detalle-sesion",
    label: "Detalle de la Sesion",
    leftIcon: <UnorderedListOutlined />,
  },
  {
    key: "grupo-relacionado",
    label: "Grupo Relacionado",
    leftIcon: <UsergroupAddOutlined />,
  },
  {
    key: "estado-paginacion",
    label: "Estado Paginacion",
    leftIcon: <SnippetsFilled />,
  },
];

export default function Workflow() {
  const navigate = useNavigate();

  return (
    <div className={styles.container} data-testid="workflow-content">
      <div data-testid="workflow-toolbar">
        <AppToolbar
          actionContent={
            <div className={styles.toolbarRow}>
              <div className={styles.toolbarLeft}>
                <span className={styles.title}>Workflow</span>
              </div>
              <div className={styles.toolbarRight}>
                <AppDropdown
                  ariaLabel="Opciones"
                  className={styles.toolbarControl}
                  trigger={
                    <AppButton variant="ghost" size="sm" fullWidth>
                      Opciones
                    </AppButton>
                  }
                  items={dropdownItems}
                />
                <AppButton className={styles.toolbarControl} variant="ghost" size="sm">
                  Actualizar
                </AppButton>
                <AppButton
                  className={styles.toolbarControl}
                  variant="ghost"
                  size="sm"
                  onClick={() => navigate("asignacion")}
                >
                  Abrir asignacion
                </AppButton>
                <AppButton
                  className={styles.toolbarControl}
                  variant="ghost"
                  size="sm"
                  onClick={() => navigate("enlace")}
                >
                  Abrir enlace
                </AppButton>
              </div>
            </div>
          }
        />
      </div>

      <AppContent
        data-testid="workflow-appcontent"
        className={styles.content}
        contentClassName={styles.contentBody}
        width="full"
        density="compact"
      >
        <div className={styles.contentWrapper}>
          <h2>Listado de Workflow</h2>
          <div className={styles.tablePlaceholder}>
            Aqui ira la tabla (AG Grid / Ant Design)
          </div>
        </div>
      </AppContent>
    </div>
  );
}
