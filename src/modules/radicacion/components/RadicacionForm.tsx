import React, { useMemo, useState } from "react";
import {
  Form,
  Select,
  Button,
  Card,
  Space,
  Tag,
  Dropdown,
  Modal,
  Checkbox,
  Col,
  DatePicker,
  Input,
  Row,
  Tooltip,
} from "antd";

import {
  InfoCircleOutlined,
  EditOutlined,
  DeleteOutlined,
  UnorderedListOutlined,
  CloseOutlined,
  CalendarOutlined,
  SearchOutlined,
  DeleteFilled,
  FileFilled,
  RocketFilled,
  OpenAIFilled,
} from "@ant-design/icons";

import styles from "../style/FormRadicacion.module.css";
import { useCamposPlantilla } from "../hooks/useCamposPlantilla";
import {
  CampoPlantillaAutoCompleteField,
  CamposPlantillaAutoCompleteRenderer,
} from "./CamposPlantillaAutoCompleteRenderer";

/* =========================================================
   MODELOS
========================================================= */

interface Usuario {
  id: number;
  nombre: string;
}

interface SelectUsuariosProps {
  label: string;
  name: string;
  opciones: { label: string; value: number }[];
  abrirInformacion: (id: number) => void;

  rules?: any; // 👈 AGREGA ESTA LÍNEA


  // 🔹 NUEVAS PROPS CONFIGURABLES
  showUserPlusButton?: boolean;
  allowEdit?: boolean;
  allowDelete?: boolean;
}

/* =========================================================
   COMPONENTE BASE CONFIGURABLE
========================================================= */

const BaseSelectUsuarios: React.FC<SelectUsuariosProps> = ({
  label,
  name,
  opciones,
  abrirInformacion,
  rules, // 👈 agregar aquí rules
  showUserPlusButton = true,
  allowEdit = true,
  allowDelete = true,
}) => {
  const [value, setValue] = useState<number[]>([]);
  const [openSelect, setOpenSelect] = useState(false);
  const [tagMenuOpen, setTagMenuOpen] = useState(false);
  const [searchText, setSearchText] = useState("");

  /**
   * Solo permite un usuario seleccionado.
   * Si se selecciona otro, reemplaza el anterior.
   */
  const handleChange = (values: number[]) => {
    const ultimo = values.slice(-1);
    setValue(ultimo);
    setOpenSelect(false);
    setSearchText("");
  };

  /**
   * Construcción dinámica del menú según configuración
   */
  const buildMenuItems = (value: number, onClose: () => void) => {
    const items: any[] = [
      {
        key: "info",
        label: (
          <Space>
            <InfoCircleOutlined />
            Información
          </Space>
        ),
        onClick: () => abrirInformacion(value),
      },
    ];

    if (allowEdit) {
      items.push({
        key: "edit",
        label: (
          <Space>
            <EditOutlined />
            Editar
          </Space>
        ),
      });
    }

    if (allowDelete) {
      items.push({
        key: "delete",
        danger: true,
        label: (
          <Space>
            <DeleteOutlined />
            Eliminar
          </Space>
        ),
        onClick: () => onClose(),
      });
    }

    return items;
  };

  /**
   * Tag personalizado con dropdown configurable
   */
  const customTagRender = (props: any) => {
    const { label, value, closable, onClose } = props;

    return (
      <Tag closable={false}>
        {label}

        <Dropdown
          menu={{ items: buildMenuItems(value, onClose) }}
          trigger={["click"]}
          onOpenChange={(open) => {
            setTagMenuOpen(open);
            if (open) setOpenSelect(false);
          }}
        >
          <UnorderedListOutlined
            style={{ marginLeft: 6, cursor: "pointer" }}
            onClick={(e) => e.stopPropagation()}
          />
        </Dropdown>

        {closable && (
          <CloseOutlined
            style={{ marginLeft: 6, cursor: "pointer" }}
            onClick={onClose}
          />
        )}
      </Tag>
    );
  };

  return (
    <Form.Item name={name} label={label} rules={rules}>
      <Space.Compact style={{ width: "100%" }}>
        <Select
          mode="multiple"
          value={value}
          showSearch
          searchValue={searchText}
          placeholder={`Escriba para buscar ${label.toLowerCase()}`}
          options={opciones}
          optionFilterProp="label"
          tagRender={customTagRender}
          open={openSelect}
          disabled={tagMenuOpen}
          autoClearSearchValue={false}
          onSearch={(text) => {
            setSearchText(text);

            if (text.length > 0 && !tagMenuOpen) {
              setOpenSelect(true);
            } else {
              setOpenSelect(false);
            }
          }}
          onChange={handleChange}
          onOpenChange={(visible) => {
            if (!visible) setOpenSelect(false);
          }}
        />

        {/* 🔹 Botón opcional */}
        {showUserPlusButton && (
          <Button
            className={styles.btnRemite}
            shape="circle"
            icon={<i className="fa-solid fa-user-plus" />}
          />
        )}
      </Space.Compact>
    </Form.Item>
  );
};

/* =========================================================
   SELECT REMITENTE
   - Con botón user plus
   - Con editar y eliminar
========================================================= */

const SelectRemitente: React.FC<SelectUsuariosProps> = (props) => {
  return (
    <BaseSelectUsuarios
      {...props}
      showUserPlusButton={true}
      allowEdit={true}
      allowDelete={true}
    />
  );
};

/* =========================================================
   SELECT DESTINATARIO
   - Sin botón user plus
   - Solo información
========================================================= */

const SelectDestinatario: React.FC<SelectUsuariosProps> = (props) => {
  return (
    <BaseSelectUsuarios
      {...props}
      showUserPlusButton={false}
      allowEdit={false}
      allowDelete={false}
    />
  );
};

/* =========================================================
   COMPONENTE PRINCIPAL
========================================================= */

const FormRadicacion: React.FC = () => {
  const [form] = Form.useForm();

  const { data: camposPlantilla, isLoading: isLoadingCamposPlantilla } =
    useCamposPlantilla();

  const usuarios: Usuario[] = [
    { id: 1, nombre: "Juan Pérez" },
    { id: 2, nombre: "María Gómez" },
    { id: 3, nombre: "Maria Victoria" },
    { id: 4, nombre: "Camila Urueta" },
    { id: 5, nombre: "Yuli Alexandra" },
    { id: 6, nombre: "Bertha Hernandez" },
    { id: 7, nombre: "Carlos Rodríguez" },
    { id: 8, nombre: "Ana Martínez" },
  ];

  const opcionesUsuarios = usuarios.map((user) => ({
    label: user.nombre,
    value: user.id,
  }));

  const [modalVisible, setModalVisible] = useState(false);
  const [usuarioSeleccionado, setUsuarioSeleccionado] =
    useState<Usuario | null>(null);

  const [resetKey, setResetKey] = useState(0);

  const abrirInformacion = (id: number) => {
    const user = usuarios.find((u) => u.id === id);
    if (user) {
      setUsuarioSeleccionado(user);
      setModalVisible(true);
    }
  };

  const camposPlantillaSafe = camposPlantilla ?? [];

  const campoTramite = useMemo(
    () =>
      camposPlantillaSafe.find(
        (campo) => campo.name_campo === "Descripcion_Documento",
      ),
    [camposPlantillaSafe],
  );

  const tramiteOptions = useMemo(() => {
    console.log(campoTramite);
    const opciones = campoTramite?.ilist_row_drowlist ?? [];

    return opciones
      .map((opcion) => {
        const anyOption = opcion as unknown as {
          id_value?: string | number;
          value_campo?: string;
          idValue?: string | number;
          Value?: string;
        };
        const value = anyOption.id_value ?? anyOption.idValue ?? "";
        const label = anyOption.value_campo ?? anyOption.Value ?? "";
        return { value, label };
      })
      .filter((opcion) => opcion.value !== "" || opcion.label !== "");
  }, [campoTramite]);

  const campoTipoRadicado = useMemo(
    () =>
      camposPlantillaSafe.find((campo) => campo.name_campo === "TipoRadicado"),
    [camposPlantillaSafe],
  );

  const campoAnexos = useMemo(
    () =>
      camposPlantillaSafe.find((campo) => campo.name_campo === "ANEXOS_COR"),
    [camposPlantillaSafe],
  );
  const campoAsunto = useMemo(
    () => camposPlantillaSafe.find((campo) => campo.name_campo === "ASUNTO"),
    [camposPlantillaSafe],
  );

  const campoFlujo = useMemo(
    () =>
      camposPlantillaSafe.find(
        (campo) => campo.name_campo === "RE_flujo_trabajo",
      ),
    [camposPlantillaSafe],
  );
  const campoFechaLimiteRespuesta = useMemo(
    () =>
      camposPlantillaSafe.find(
        (campo) => campo.name_campo === "FECHALIMITERESPUESTA",
      ),
    [camposPlantillaSafe],
  );

  const tipoRadicadoOptions = useMemo(() => {
    const opciones = campoTipoRadicado?.ilist_row_drowlist ?? [];
    const mapped = opciones.map((opcion) => {
      const anyOption = opcion as unknown as {
        id_value?: string | number | null;
        value_campo?: string | null;
        idValue?: string | number | null;
        Value?: string | null;
      };
      const value = anyOption.idValue ?? anyOption.id_value ?? "";
      const label = anyOption.Value ?? anyOption.value_campo ?? "";
      return { value, label };
    });

    return [{ value: "", label: "Seleccionar" }, ...mapped].filter(
      (opcion) => opcion.value !== "" || opcion.label !== "",
    );
  }, [campoTipoRadicado]);

  const tipoRadicadoLabel = campoTipoRadicado?.aleas_campo ?? "Tipo de Radicado";
  const tipoRadicadoTitle = campoTipoRadicado?.title_control ?? "";
  const tipoRadicadoTooltip = campoTipoRadicado?.tooltipAyuda ?? "";
  const tipoRadicadoTooltipId = tipoRadicadoTooltip
    ? "pl-radicacion-spe-tooltip-TipoRadicado"
    : undefined;

  const tipoRadicadoLabelNode = (
    <span title={tipoRadicadoTitle}>
      {tipoRadicadoLabel}
      {tipoRadicadoTooltip ? (
        <Tooltip title={tipoRadicadoTooltip}>
          <span
            className={styles["tooltip-ayuda"]}
            role="button"
            tabIndex={0}
            aria-label={`Mostrar ayuda para ${tipoRadicadoLabel}`}
            aria-describedby={tipoRadicadoTooltipId}
            data-tooltip-id={tipoRadicadoTooltipId}
          >
            <InfoCircleOutlined />
          </span>
        </Tooltip>
      ) : null}
    </span>
  );

  const flujoLabel = campoFlujo?.aleas_campo ?? "Flujo Trámite";
  const flujoTitle = campoFlujo?.title_control ?? "";
  const flujoTooltip = campoFlujo?.tooltipAyuda ?? "";
  const flujoTooltipId = flujoTooltip
    ? "pl-radicacion-spe-tooltip-RE_flujo_trabajo"
    : undefined;

  const flujoLabelNode = (
    <span title={flujoTitle}>
      {flujoLabel}
      {flujoTooltip ? (
        <Tooltip title={flujoTooltip}>
          <span
            className={styles["tooltip-ayuda"]}
            role="button"
            tabIndex={0}
            aria-label={`Mostrar ayuda para ${flujoLabel}`}
            aria-describedby={flujoTooltipId}
            data-tooltip-id={flujoTooltipId}
          >
            <InfoCircleOutlined />
          </span>
        </Tooltip>
      ) : null}
    </span>
  );

  const tramiteLabel = campoTramite?.aleas_campo ?? "Trámite";
  const tramiteTitle = campoTramite?.title_control ?? "";
  const tramiteTooltip = campoTramite?.tooltipAyuda ?? "";
  const tramiteTooltipId = tramiteTooltip
    ? "pl-radicacion-spe-tooltip-Descripcion_Documento"
    : undefined;

  const tramiteLabelNode = (
    <span title={tramiteTitle}>
      {tramiteLabel}
      {tramiteTooltip ? (
        <Tooltip title={tramiteTooltip}>
          <span
            className={styles["tooltip-ayuda"]}
            role="button"
            tabIndex={0}
            aria-label={`Mostrar ayuda para ${tramiteLabel}`}
            aria-describedby={tramiteTooltipId}
            data-tooltip-id={tramiteTooltipId}
          >
            <InfoCircleOutlined />
          </span>
        </Tooltip>
      ) : null}
    </span>
  );

  const fechaLimiteLabel =
    campoFechaLimiteRespuesta?.aleas_campo ?? "Fecha Límite Respuesta";
  const fechaLimiteTitle =
    campoFechaLimiteRespuesta?.title ??
    campoFechaLimiteRespuesta?.title_control ??
    "";
  const fechaLimiteTooltip = campoFechaLimiteRespuesta?.tooltipAyuda ?? "";
  const fechaLimiteTooltipId = fechaLimiteTooltip
    ? "pl-radicacion-spe-tooltip-FECHALIMITERESPUESTA"
    : undefined;

  const fechaLimiteLabelNode = (
    <span title={fechaLimiteTitle}>
      {fechaLimiteLabel}
      {fechaLimiteTooltip ? (
        <Tooltip title={fechaLimiteTooltip}>
          <span
            className={styles["tooltip-ayuda"]}
            role="button"
            tabIndex={0}
            aria-label={`Mostrar ayuda para ${fechaLimiteLabel}`}
            aria-describedby={fechaLimiteTooltipId}
            data-tooltip-id={fechaLimiteTooltipId}
          >
            <InfoCircleOutlined />
          </span>
        </Tooltip>
      ) : null}
    </span>
  );

  return (
    <div className={styles.container}>
      <div className={styles.content}>

        <Form layout="vertical" form={form}>
          {/* ================= OPCIONES ================= */}
          <Card
            className={styles.modernCard}
            title={
              <Space className={styles.cardTitle}>
                <i className="fa-solid fa-file-circle-plus" />
                Opciones de Radicación
              </Space>
            }
            size="small"
            style={{ marginBottom: 24 }}
          >
            <Row gutter={16}>
              <Col xs={24} md={4}>
                <Form.Item name="nuevoRadicado" valuePropName="checked">
                  <Checkbox>Nuevo Radicado</Checkbox>
                </Form.Item>
              </Col>

              <Col xs={24} md={5}>
                <Form.Item name="relacionarRadicado" valuePropName="checked">
                  <Checkbox>Relacionar a Radicado</Checkbox>
                </Form.Item>
              </Col>

              <Col xs={24} md={6}>
                <Form.Item name="buscarRadicado" label="Buscar Radicado">
                  <Input suffix={<SearchOutlined />} />
                </Form.Item>
              </Col>

              <Col xs={24} md={5}>
                <Form.Item
                  name="expedienteRelacionado"
                  label="Expediente Relacionado"
                >
                  <Input />
                </Form.Item>
              </Col>

              <Col xs={24} md={4}>
                <Form.Item label=" " colon={false}>
                  <Button
                    className={styles.btnConsulta}
                    icon={<SearchOutlined />}
                    block
                  >
                    Consulta
                  </Button>
                </Form.Item>
              </Col>
            </Row>
          </Card>
          {/* ====================== MEDIO DE RECEPCIÓN ====================== */}
          <Card  data-ident="pl-radicacion-spe-card"
            className={styles.modernCard}
            title={
              <Space className={styles.cardTitle}>
                <i className="fa-solid fa-inbox" />
                Medio de Recepción del Trámite
              </Space>
            }
            size="small"
            style={{ marginBottom: 24 }}
          >
            <Row gutter={16}>
              <Col xs={24} md={8}>
                <Form.Item
                  label={tipoRadicadoLabelNode}
                  data-ident="pl-radicacion-spe-TipoRadicado"
                  rules={[{ required: true, message: "Seleccione una opción" }]}
                >
                  <Select
                    placeholder="Seleccione"
                    options={tipoRadicadoOptions}
                    data-ident="pl-radicacion-spe-TipoRadicado"
                    aria-describedby={tipoRadicadoTooltipId}
                    data-testid="ra_tipo_radicado_select"
                  />
                </Form.Item>
              </Col>

              <Col xs={24} md={8}>
                {campoAnexos ? (
                  <CampoPlantillaAutoCompleteField campo={campoAnexos} />
                ) : (
                  <Form.Item label="Anexos Radicado" name="anexos">
                    <Input
                      data-ident="pl-radicacion-spe-ANEXOS_COR"
                      placeholder="Descripción del anexo"
                    />
                  </Form.Item>
                )}
              </Col>
            </Row>
          </Card>
          {/* ====================== CLASIFICACIÓN ====================== */}
          <Card
            className={styles.modernCard}
            title={
              <Space className={styles.cardTitle}>
                <i className="fa-solid fa-folder-tree" />
                Clasificación del Trámite
              </Space>
            }
            size="small"
            style={{ marginBottom: 24 }}
          >
            <Row gutter={16}>
              <Col xs={24} md={8}>
                <Form.Item
                  label={tramiteLabelNode}
                  name="tramite"
                  id="Descripcion_Documento"
                  data-ident="pl-radicacion-spe-Descripcion_Documento"
                  rules={[{ required: true, message: "Seleccione trámite" }]}
                >
                  <Select
                    placeholder="Seleccione"
                    options={tramiteOptions}
                    loading={isLoadingCamposPlantilla}
                    data-testid="ra_tipo_tramite_select"
                    disabled={campoTramite?.disable_campo === 1}
                    aria-describedby={tramiteTooltipId}
                  />
                </Form.Item>
              </Col>

              <Col xs={24} md={8}>
                <Form.Item label={flujoLabelNode} name="flujo">
                  <Select
                    placeholder="Seleccione"
                    data-ident="pl-radicacion-spe-RE_flujo_trabajo"
                    disabled={campoFlujo?.disable_campo === 1}
                    aria-describedby={flujoTooltipId}
                  />
                </Form.Item>
              </Col>

              <Col xs={24} md={8}>
                <Form.Item label={fechaLimiteLabelNode} name="fechaLimite">
                  <DatePicker
                    style={{ width: "100%" }}
                    suffixIcon={<CalendarOutlined />}
                    data-ident="pl-radicacion-spe-FECHALIMITERESPUESTA"
                    data-testid="ra_fecha_limite_picker"
                    aria-describedby={fechaLimiteTooltipId}
                  />
                </Form.Item>
              </Col>
            </Row>

            {campoAsunto ? (
              <CampoPlantillaAutoCompleteField campo={campoAsunto} />
            ) : (
              <Form.Item
                label="Asunto"
                name="asunto"
                data-ident="pl-radicacion-spe-ASUNTO"
                rules={[{ required: true, message: "Ingrese asunto" }]}
              >
                <Input placeholder="Descripción del asunto" />
              </Form.Item>
            )}
          </Card>

          {/*=========================REMITENTE============================*/}
          <Card
            className={styles.modernCard}
            title={
              <Space className={styles.cardTitle}>
                <i className="fa-solid fa-user" />
                Caracterización del Remitente
              </Space>
            }
            size="small"
            style={{ marginBottom: 24 }}
          >
            <SelectRemitente
              rules={[{ required: true, message: "Seleccione remitente" }]}
              key={`remitente-${resetKey}`}
              label="Remitente"
              name="remitente"
              data-ident="pl-radicacion-spe-REMITENTE_COR"
              opciones={opcionesUsuarios}
              abrirInformacion={abrirInformacion}
            />
          </Card>
          {/*=========================DESTINATARIO============================*/}
          <Card
            className={styles.modernCard}
            title={
              <Space className={styles.cardTitle}>
                <i className="fa-solid fa-user" />
                Caracterización del Destinatario
              </Space>
            }
            size="small"
            style={{ marginBottom: 24 }}
          >
            <SelectDestinatario
              rules={[{ required: true, message: "Seleccione destinatario" }]}
              key={`destinatario-${resetKey}`}
              label="Destinatario"
              name="destinatario"
              data-ident="pl-radicacion-spe-Destinatario_Cor"
              opciones={opcionesUsuarios}
              abrirInformacion={abrirInformacion}
            />
          </Card>

          {camposPlantillaSafe.length > 0 ? (
            <CamposPlantillaAutoCompleteRenderer
              camposPlantilla={camposPlantillaSafe}
            />
          ) : null}
        </Form>
      </div>


      <Modal
        title="Información del Usuario"
        open={modalVisible}
        onCancel={() => setModalVisible(false)}
        footer={null}
      >
        {usuarioSeleccionado && (
          <>
            <p><strong>ID:</strong> {usuarioSeleccionado.id}</p>
            <p><strong>Nombre:</strong> {usuarioSeleccionado.nombre}</p>
          </>
        )}
      </Modal>

      {/* ================= FOOTER ================= */}
      <div className={styles.footer}>
        {/* IZQUIERDA */}
        <Button
          icon={<OpenAIFilled />}
          className={styles.btnRad}
          onClick={() => {
            form.resetFields();
            setResetKey(prev => prev + 1);
          }}
        >
          Documentos IA
        </Button>

        {/* DERECHA */}
        <div className={styles.rightGroup}>
          <Button
            icon={<DeleteFilled />}
            className={styles.btnClear}
            onClick={() => {
              form.resetFields();
              setResetKey(prev => prev + 1);
            }}
          >
            Limpiar
          </Button>

          <Button
            icon={<RocketFilled />}
            className={styles.btnPending}
          >
            Enviar a Pendientes
          </Button>

          <Button
            icon={<FileFilled />}
            className={styles.btnRad}
            onClick={() => form.submit()}
          >
            Radicar
          </Button>
        </div>
      </div>
    </div>
  );
};

export default FormRadicacion;
