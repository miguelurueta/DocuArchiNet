import { useRef } from "react";
import { Button, List, Checkbox, Divider, Dropdown, Layout } from "antd";
import {
  PrinterFilled,
  SaveFilled,
  InfoCircleFilled,
  FileTextFilled,
  EditFilled,
  FolderOpenFilled,
  SwapOutlined,
  DeleteFilled,
  DownCircleFilled,
} from "@ant-design/icons";
import {
  DigitalizacionDocumentalWorkspace,
} from "../../digitalizacion";
import { useAppDigitalizadorScannerClient } from "../../../app/Components/UI/AppDigitalizador/hooks/useAppDigitalizadorScannerClient";

import styles from "../style/capdocument.module.css";

const { Sider } = Layout;

const CapDocument = () => {
  const dynamsoftLicenseFromEnv = import.meta.env.VITE_DYNAMSOFT_LICENSE_KEY;
  const scannerClient = useAppDigitalizadorScannerClient({
    licenciaDynamsoft: dynamsoftLicenseFromEnv,
  });
  const documentMenuItems = [
    {
      key: "1",
      icon: <FileTextFilled />,
      label: "Cambiar Tipología",
    },
    {
      key: "2",
      icon: <EditFilled />,
      label: "Firma Digital",
    },
    {
      key: "3",
      icon: <FolderOpenFilled />,
      label: "Versiones del Documento",
    },
    {
      key: "4",
      icon: <SwapOutlined />,
      label: "Reemplazar Documento",
    },
    {
      type: "divider" as const,
    },
    {
      key: "5",
      icon: <DeleteFilled />,
      label: <span style={{ color: "red" }}>Eliminar</span>,
    },
  ];

  const digitalizadorContext = {
    modo: "crear" as const,
    nombreGabinete: "CAPDOCUMENT",
    radicado: "CAPDOCUMENT",
  };

  const handleDigitalizadorCompleted = () => {};
  const handleDigitalizadorError = () => {};
  const toolbarHostRef = useRef<HTMLDivElement>(null);

  return (
    <div className={styles.container}>
      <div className={styles.topToolbar}>
        <Button className={styles.btnAcept} icon={<PrinterFilled />}>
          Imprimir Rótulo
        </Button>
        <Button className={styles.btnAcept} icon={<SaveFilled />}>
          Guardar Rótulo
        </Button>
        <Button className={styles.btnAcept} icon={<InfoCircleFilled />}>
          Detalle Radicado
        </Button>
      </div>
      <div ref={toolbarHostRef} className={styles.toolbarHost} />

      <Layout className={styles.mainLayout}>
        <div className={styles.centerPanel}>
          <DigitalizacionDocumentalWorkspace
            scannerClient={scannerClient}
            context={digitalizadorContext}
            onCompleted={handleDigitalizadorCompleted}
            onError={handleDigitalizadorError}
            toolbarHost={toolbarHostRef}
          />
        </div>

        <Sider width={300} className={styles.rightPanel}>
          <div className={styles.docHeader}>
            <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
              <Checkbox />
              <span>Documentos: 4</span>
            </div>

            <div style={{ display: "flex", gap: 6 }}>
              <Button type="text" size="small" icon={<InfoCircleFilled />} />
              <Button type="text" size="small" icon={<DeleteFilled />} />
              <Button type="text" size="small" icon={<EditFilled />} />
            </div>
          </div>

          <Divider />

          <List
            dataSource={["Factura.pdf", "Factura.pdf"]}
            renderItem={(item) => (
              <List.Item className={styles.docItem}>
                <div className={styles.docRow}>
                  <Checkbox>{item}</Checkbox>

                  <div className={styles.docActions}>
                    <Button shape="circle" size="small" icon={<FileTextFilled />} />
                    <Dropdown
                      menu={{ items: documentMenuItems }}
                      trigger={[
                        "click",
                      ]}
                      placement="bottomRight"
                    >
                      <Button
                        shape="circle"
                        size="small"
                        icon={<DownCircleFilled />}
                      />
                    </Dropdown>
                  </div>
                </div>
              </List.Item>
            )}
          />
        </Sider>
      </Layout>
    </div>
  );
};

export default CapDocument;
