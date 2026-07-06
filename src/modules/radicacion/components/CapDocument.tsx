import { useRef } from "react";
import { Alert, Button, Divider, Empty, Layout } from "antd";
import {
  InfoCircleFilled,
  PrinterFilled,
  SaveFilled,
} from "@ant-design/icons";
import { DigitalizacionDocumentalWorkspace } from "../../digitalizacion";
import { useAppDigitalizadorScannerClient } from "../../../app/Components/UI/AppDigitalizador/hooks/useAppDigitalizadorScannerClient";
import { useRadicacionDocumentalContext } from "../hooks/useRadicacionDocumentalContext";

import styles from "../style/capdocument.module.css";

const { Sider } = Layout;

const CapDocument = () => {
  const {
    idEstadoRadicado,
    idRadicado,
    consecutivoRadicado,
    contextoDocumental,
  } = useRadicacionDocumentalContext();
  const dynamsoftLicenseFromEnv = import.meta.env.VITE_DYNAMSOFT_LICENSE_KEY;
  const scannerClient = useAppDigitalizadorScannerClient({
    licenciaDynamsoft: dynamsoftLicenseFromEnv,
  });
  const toolbarHostRef = useRef<HTMLDivElement>(null);

  const nombreGabinete = contextoDocumental?.nombreGabinete ?? null;
  const radicado =
    consecutivoRadicado ??
    (idRadicado ? String(idRadicado) : null) ??
    (idEstadoRadicado ? String(idEstadoRadicado) : null);

  const canInitializeDigitalizador = Boolean(nombreGabinete && radicado);

  const digitalizadorContext = canInitializeDigitalizador
    ? {
        modo: "crear" as const,
        nombreGabinete: String(nombreGabinete),
        radicado: String(radicado),
      }
    : null;

  const handleDigitalizadorCompleted = () => {};
  const handleDigitalizadorError = () => {};

  return (
    <div className={styles.container}>
      <div className={styles.topToolbar}>
        <Button className={styles.btnAcept} icon={<PrinterFilled />}>
          Imprimir rótulo
        </Button>
        <Button className={styles.btnAcept} icon={<SaveFilled />}>
          Guardar rótulo
        </Button>
        <Button className={styles.btnAcept} icon={<InfoCircleFilled />}>
          Detalle radicado
        </Button>
      </div>
      <div ref={toolbarHostRef} className={styles.toolbarHost} />

      <Layout className={styles.mainLayout}>
        <div className={styles.centerPanel}>
          {digitalizadorContext ? (
            <DigitalizacionDocumentalWorkspace
              scannerClient={scannerClient}
              context={digitalizadorContext}
              onCompleted={handleDigitalizadorCompleted}
              onError={handleDigitalizadorError}
              toolbarHost={toolbarHostRef}
            />
          ) : (
            <Alert
              type="info"
              showIcon
              title="Contexto documental incompleto"
              description="La captura documental estará disponible cuando el backend entregue el gabinete y radicado activos."
            />
          )}
        </div>

        <Sider width={300} className={styles.rightPanel}>
          <div className={styles.docHeader}>
            <span>Documentos</span>
            <Button type="text" size="small" icon={<InfoCircleFilled />} />
          </div>

          <Divider />

          <Empty
            image={Empty.PRESENTED_IMAGE_SIMPLE}
            description="No hay documentos cargados para este trámite."
          />
        </Sider>
      </Layout>
    </div>
  );
};

export default CapDocument;
