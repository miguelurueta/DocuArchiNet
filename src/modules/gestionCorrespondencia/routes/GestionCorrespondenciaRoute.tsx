import { CloseOutlined } from "@ant-design/icons";
import type { ReactNode } from "react";
import { useNavigate } from "react-router-dom";
import { AppButton } from "../../../app/Components/UI/AppButton";
import GestionCorrespondenciaRoutePage from "../pages/GestionCorrespondenciaRoutePage";
import styles from "../style/GestionCorrespondenciaRoute.module.css";

interface GestionCorrespondenciaRouteProps {
  detailContent?: ReactNode;
}

export default function GestionCorrespondenciaRoute({
  detailContent,
}: GestionCorrespondenciaRouteProps) {
  const navigate = useNavigate();
  const hasDetail = Boolean(detailContent);

  const handleClose = () => {
    navigate("/dashboard/gestion-correspondencia");
  };

  return (
    <section
      className={`${styles.shell} ${hasDetail ? styles.shellWithDetail : ""}`.trim()}
      data-testid="gestion-correspondencia-route-shell"
    >
      <div className={styles.mainRegion} data-testid="gestion-correspondencia-main-region">
        <GestionCorrespondenciaRoutePage />
      </div>

      {hasDetail ? (
        <aside
          className={styles.detailRegion}
          aria-label="Panel contextual de gestion de correspondencia"
          data-testid="gestion-correspondencia-detail-region"
        >
          <header className={styles.detailHeader}>
            <div className={styles.detailHeaderCopy}>
              <span className={styles.detailEyebrow}>Vista secundaria</span>
              <h2 className={styles.detailTitle}>Respuesta contextual</h2>
            </div>

            <AppButton
              aria-label="Cerrar panel contextual"
              icon={<CloseOutlined />}
              size="sm"
              tooltip="Cerrar panel contextual"
              variant="ghost"
              onClick={handleClose}
            />
          </header>

          <div className={styles.detailBody}>{detailContent}</div>
        </aside>
      ) : null}
    </section>
  );
}
