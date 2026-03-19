import {
  Layout,
  Button,
  Card,
  List,
  Checkbox,
  Divider,
  Dropdown,
} from "antd";
import {
  PrinterFilled,
  SaveFilled,
  InfoCircleFilled,
  // SendFilled,
  ScanOutlined,
  LeftCircleFilled,
  RightCircleFilled,
  ZoomInOutlined,
  ZoomOutOutlined,
  DownCircleFilled,
  FileTextFilled,
  EditFilled,
  FolderOpenFilled,
  SwapOutlined,
  DeleteFilled,
  ReloadOutlined,
  StopFilled,
  DragOutlined,
  ExpandOutlined,
  StepBackwardFilled,
  StepForwardFilled,
  SearchOutlined,
  LinkOutlined,
  FileExcelFilled,
} from "@ant-design/icons";

import styles from "../style/capdocument.module.css";

const { Content, Sider } = Layout;

const CapDocument = () => {

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

  const printMenu = [
    { key: "1", label: "Imprimir documento actual" },
    { key: "2", label: "Imprimir todas las páginas" },
  ];

  const saveMenu = [
    { key: "1", label: "Guardar como PDF" },
    { key: "2", label: "Descargar original" },
  ];

  const zoomMenu = [
    { key: "1", label: "50%" },
    { key: "2", label: "75%" },
    { key: "3", label: "100%" },
    { key: "4", label: "150%" },
    { key: "5", label: "200%" },
  ];

  const linkMenu = [
    { key: "1", label: "Copiar enlace" },
    { key: "2", label: "Abrir en nueva pestaña" },
  ];

  return (
    <div className={styles.container}>
      <div className={styles.topToolbar}>
        <Button className={styles.btnAcept} icon={<PrinterFilled />}>Imprimir Rótulo</Button>
        <Button className={styles.btnAcept} icon={<SaveFilled />}>Guardar Rótulo</Button>
        <Button className={styles.btnAcept} icon={<InfoCircleFilled />}>Detalle Radicado</Button>
        {/* <Button className={styles.btnAcept} icon={<SendFilled />}>Enviar a Flujo</Button>cbcbuc z */}
        <Button className={styles.btnAcept} icon={<ScanOutlined />}>Scanner</Button>
      </div>

      <div className={styles.viewerToolbar}>

        <Dropdown menu={{ items: printMenu }} trigger={["click"]}>
          <Button>
            <PrinterFilled />
            <DownCircleFilled style={{ fontSize: 10, marginLeft: 4 }} />
          </Button>
        </Dropdown>

        <Dropdown menu={{ items: saveMenu }} trigger={["click"]}>
          <Button>
            <SaveFilled />
            <DownCircleFilled style={{ fontSize: 10, marginLeft: 4 }} />
          </Button>
        </Dropdown>

        <Button icon={<ReloadOutlined />} />
        <Button icon={<StopFilled />} />
        <Button icon={<DragOutlined />} />
        <Button icon={<ExpandOutlined />} />

        <Button icon={<StepBackwardFilled />} />
        <Button icon={<LeftCircleFilled />} />
        <Button icon={<RightCircleFilled />} />
        <Button icon={<StepForwardFilled />} />

        <Button icon={<ZoomInOutlined />} />
        <Button icon={<ZoomOutOutlined />} />

        <Button icon={<FileExcelFilled />} />
        <Button icon={<SearchOutlined />} />

        <Dropdown menu={{ items: zoomMenu }} trigger={["click"]}>
          <Button>
            -1
            <DownCircleFilled style={{ fontSize: 10, marginLeft: 4 }} />
          </Button>
        </Dropdown>

        <Dropdown menu={{ items: linkMenu }} trigger={["click"]}>
          <Button>
            <LinkOutlined />
            <DownCircleFilled style={{ fontSize: 10, marginLeft: 4 }} />
          </Button>
        </Dropdown>

      </div>

      <Layout className={styles.mainLayout}>
        <Sider width={280} className={styles.leftPanel}>
          <h3 className={styles.sectionTitle}>Panel digitalización</h3>
          <p className={styles.subtitle}>
            Aquí se mostrará la información detallada del registro seleccionado.
          </p>

          <Card className={styles.innerCard}>
            <h4>Detalles Adicionales</h4>
            <p>Correo: juanperez@example.com</p>
            <p>Último acceso: 28/10/2025</p>
            <p>Rol: Administrador</p>
          </Card>
        </Sider>

        <Content className={styles.centerPanel}>
          <h2 className={styles.sectionTitle}>Aquí va el Contenido</h2>
          <p className={styles.subtitle}>
            Aquí se mostrará la información detallada.
          </p>

          <Card className={styles.innerCard}>
            <h4>Información General</h4>
            <p><b>Nombre:</b> Juan Pérez</p>
            <p><b>Estado:</b> Activo</p>
            <p><b>Descripción:</b> Usuario activo del sistema.</p>
          </Card>

          <Card className={styles.innerCard}>
            <h4>Detalles Adicionales</h4>
            <p>Correo: juanperez@example.com</p>
            <p>Último acceso: 28/10/2025</p>
            <p>Rol: Administrador</p>
          </Card>
        </Content>

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
                    <Button
                      shape="circle"
                      size="small"
                      icon={<FileTextFilled />}
                    />

                    <Dropdown
                      menu={{ items: documentMenuItems }}
                      trigger={["click"]}
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
