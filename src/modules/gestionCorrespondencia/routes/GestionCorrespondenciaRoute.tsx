import { LeftOutlined } from "@ant-design/icons";
import type { ReactNode } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { AppButton } from "../../../app/Components/UI/AppButton";
import { useEstructuraRespuestaIdTarea } from "../hooks/useEstructuraRespuestaIdTarea";
import GestionCorrespondenciaRoutePage from "../pages/GestionCorrespondenciaRoutePage";
import styles from "../style/GestionCorrespondenciaRoute.module.css";

interface GestionCorrespondenciaRouteProps {
  detailContent?: ReactNode;
}

export default function GestionCorrespondenciaRoute({
  detailContent,
}: GestionCorrespondenciaRouteProps) {
  const navigate = useNavigate();
  const params = useParams();
  const parsedId = Number.parseInt(params.id ?? "", 10);
  const hasDetail = Boolean(detailContent);
  const { estrucTuraRespuesta, loading } = useEstructuraRespuestaIdTarea(
    hasDetail && Number.isFinite(parsedId) ? parsedId : undefined,
  );

  const metadata = [
    { label: "Radicado", value: loading ? "..." : (estrucTuraRespuesta?.Radicado ?? "-") },
    {
      label: "Remitente",
      value: loading ? "..." : (estrucTuraRespuesta?.Destinatario ?? "-"),
    },
    { label: "Trámite", value: estrucTuraRespuesta?.TramiteDocumento ?? "-" },
  ];

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
          aria-label="Panel superpuesto de gestion de correspondencia"
          data-testid="gestion-correspondencia-detail-region"
        >
          <header className={styles.detailHeader}>
            <div className={styles.detailHeaderStart}>
              <AppButton
                aria-label="Volver a la bandeja"
                className={styles.detailReturnAction}
                icon={<LeftOutlined />}
                size="sm"
                tooltip="Volver a la bandeja"
                variant="ghost"
                onClick={handleClose}
              />

              <div className={styles.detailMeta} aria-label="Metadata de la respuesta">
                {metadata.map((item) => (
                  <span className={styles.detailMetaItem} key={item.label}>
                    <strong>{item.label}:</strong>
                    <span title={item.value}>{item.value}</span>
                  </span>
                ))}
              </div>
            </div>
          </header>

          <div className={styles.detailBody}>{detailContent}</div>
        </aside>
      ) : null}
    </section>
  );
}
