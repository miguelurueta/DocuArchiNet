import React, { useEffect, useMemo, useState } from "react";
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
  OpenAIFilled,
} from "@ant-design/icons";

import styles from "../style/FormRadicacion.module.css";
import { useAutocompleteCamposPlantilla } from "../hooks/useAutocompleteCamposPlantilla";
import { useFlujosRelacionadosTramite } from "../hooks/useFlujosRelacionadosTramite";
import { useRadicacionFormReset } from "../hooks/useRadicacionFormReset";
import {
  useEstructuraRelacionTipoRestriccion,
} from "../hooks/useEstructuraRelacionTipoRestriccion";
import {
  type CDeRelacionEstadoRetriccionDto,
  C_DE_RELACION_ESTADO_RETRICCION_DESTINATARIO_DEFAULT,
} from "../models/CDeRelacionEstadoRetriccionDto";
import {
  CampoPlantillaAutoCompleteField,
  CamposPlantillaAutoCompleteRenderer,
} from "./CamposPlantillaAutoCompleteRenderer";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";
import type { PlantillaRadicadoDTO } from "../models/PlantillaRadicadoDTO";

export const C_DE_RELACION_ESTADO_RETRICCION_DTO_DEFAULT =
  C_DE_RELACION_ESTADO_RETRICCION_DESTINATARIO_DEFAULT;

/* =========================================================
   MODELOS
========================================================= */

interface Usuario {
  id: number;
  nombre: string;
}

interface SelectUsuariosProps {
  label: React.ReactNode;
  name: string;
  opciones: { label: string; value: number }[];
  abrirInformacion: (id: number) => void;
  rules?: any;
  ariaLabel?: string;
  selectDataIdent?: string;
  formItemDataIdent?: string;
  selectDisabled?: boolean;
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
  rules,
  ariaLabel,
  selectDataIdent,
  formItemDataIdent,
  selectDisabled = false,
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

  const placeholderName = (ariaLabel ?? name).toLowerCase();

  return (
    <Form.Item
      name={name}
      label={label}
      rules={rules}
      data-ident={formItemDataIdent}
    >
      <Space.Compact style={{ width: "100%" }}>
        <Select
          mode="multiple"
          value={value}
          showSearch
          searchValue={searchText}
          placeholder={`Escriba para buscar ${placeholderName}`}
          options={opciones}
          optionFilterProp="label"
          tagRender={customTagRender}
          open={openSelect}
          disabled={tagMenuOpen || selectDisabled}
          data-ident={selectDataIdent}
          aria-label={ariaLabel}
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

const resolveCampoIdScript = (campo: CampoPlantillaDTO): number | undefined => {
  const nestedId = campo.TomPParameterTomSelelect?.id_escript;
  if (typeof nestedId === "number" && Number.isFinite(nestedId)) {
    return nestedId;
  }
  const anyCampo = campo as unknown as { id_escript?: number | null };
  if (typeof anyCampo.id_escript === "number" && Number.isFinite(anyCampo.id_escript)) {
    return anyCampo.id_escript;
  }
  return undefined;
};

interface SelectRemitenteTokenProps {
  campo: CampoPlantillaDTO;
  onOpenInfo: (payload: { id: number; nombre: string }) => void;
}

const SelectRemitenteToken: React.FC<SelectRemitenteTokenProps> = ({
  campo,
  onOpenInfo,
}) => {
  const [value, setValue] = useState<Array<{ value: string; label: React.ReactNode }>>([]);
  const [openSelect, setOpenSelect] = useState(false);
  const [tagMenuOpen, setTagMenuOpen] = useState(false);
  const [searchText, setSearchText] = useState("");

  const campoIdScript = resolveCampoIdScript(campo);
  const shouldQuery = searchText.trim().length > 0;
  const { data, isLoading, isFetching } = useAutocompleteCamposPlantilla(
    shouldQuery
      ? {
          TextoBuscado: searchText,
          defaultDbAlias: "",
          tbl_control: campo.tbl_control ?? "",
          name_campo: "REMITENTE_COR",
          ...(campoIdScript !== undefined ? { idScript: campoIdScript } : {}),
        }
      : null,
    shouldQuery,
  );

  const options = data.map((item, index) => ({
    value: item.idValue ?? `${item.texValue}-${index}`,
    label: item.texValue,
  }));
  const remitenteLabel = campo.aleas_campo ?? "Remitente";
  const remitenteTitle = campo.title_control ?? "";
  const remitenteTooltip = campo.tooltipAyuda ?? "";
  const remitenteTooltipId = remitenteTooltip
    ? "pl-radicacion-spe-tooltip-REMITENTE_COR"
    : undefined;

  const handleChange = (values: Array<{ value: string; label: React.ReactNode }>) => {
    const ultimo = values.slice(-1);
    setValue(ultimo);
    setOpenSelect(false);
    setSearchText("");
  };

  const tagRender = (props: {
    label: React.ReactNode;
    value: string;
    closable: boolean;
    onClose: () => void;
  }) => {
    const { label, value, closable, onClose } = props;
    return (
      <Tag closable={false}>
        {label}
        <Dropdown
          menu={{
            items: [
              {
                key: "info",
                label: (
                  <Space>
                    <InfoCircleOutlined />
                    Información
                  </Space>
                ),
                onClick: () => {
                  onOpenInfo({
                    id: Number(value) || 0,
                    nombre: String(label ?? ""),
                  });
                },
              },
            ],
          }}
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
    <Form.Item
      name="remitente"
      label={
        <span title={remitenteTitle}>
          {remitenteLabel}
          {remitenteTooltip ? (
            <Tooltip title={remitenteTooltip}>
              <span
                className={`${styles["tooltip-ayuda"]} tooltip-ayuda`}
                role="button"
                tabIndex={0}
                aria-label={`Mostrar ayuda para ${remitenteLabel}`}
                aria-describedby={remitenteTooltipId}
                data-tooltip-id={remitenteTooltipId}
              >
                <InfoCircleOutlined />
              </span>
            </Tooltip>
          ) : null}
        </span>
      }
      rules={
        campo.obligatorio_campo === 1
          ? [{ required: true, message: "Seleccione remitente" }]
          : undefined
      }
      data-ident="pl-radicacion-spe-REMITENTE_COR"
    >
      <Space.Compact style={{ width: "100%" }} id="remitente">
        <Select
          mode="multiple"
          labelInValue
          value={value}
          showSearch
          searchValue={searchText}
          placeholder="Escriba para buscar remitente"
          options={options}
          optionFilterProp="label"
          tagRender={tagRender}
          open={openSelect}
          disabled={tagMenuOpen || campo.disable_campo === 1}
          autoClearSearchValue={false}
          loading={isLoading || isFetching}
          maxCount={1}
          data-ident="pl-radicacion-spe-REMITENTE_COR"
          aria-label={remitenteLabel}
          aria-describedby={remitenteTooltipId}
          aria-required={campo.obligatorio_campo === 1}
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
      </Space.Compact>
    </Form.Item>
  );
};

interface SelectDestinatarioTokenProps {
  campo: CampoPlantillaDTO;
  onOpenInfo: (payload: { id: number; nombre: string }) => void;
  selectDisabledByRestriction: boolean;
  relacionEstadoRestriccionDestinatario: CDeRelacionEstadoRetriccionDto;
  selectedTramiteId: string | null;
}

const SelectDestinatarioToken: React.FC<SelectDestinatarioTokenProps> = ({
  campo,
  onOpenInfo,
  selectDisabledByRestriction,
  relacionEstadoRestriccionDestinatario,
  selectedTramiteId,
}) => {
  const [value, setValue] = useState<Array<{ value: string; label: React.ReactNode }>>([]);
  const [openSelect, setOpenSelect] = useState(false);
  const [tagMenuOpen, setTagMenuOpen] = useState(false);
  const [searchText, setSearchText] = useState("");
  const [clickAutocompleteActive, setClickAutocompleteActive] = useState(false);

  const campoIdScript = resolveCampoIdScript(campo);
  const hasSelectedTramite = String(selectedTramiteId ?? "").trim().length > 0;
  const comportamientoCampo = String(campo.ComportamientoCampo ?? "")
    .trim()
    .toUpperCase();
  const canActivateAutocompleteOnClick = comportamientoCampo === "AUTOCOMPLETE";
  const normalizedSearchText = searchText.trim();
  const hasRestriccionActiva =
    relacionEstadoRestriccionDestinatario.IdTipoRestriccion !== 0;
  const textoBuscado =
    clickAutocompleteActive &&
    normalizedSearchText.length === 0 &&
    hasRestriccionActiva
      ? "*.*"
      : searchText;
  const shouldQuery =
    hasSelectedTramite &&
    (normalizedSearchText.length > 0 || clickAutocompleteActive);
  const { data, isLoading, isFetching, error } = useAutocompleteCamposPlantilla(
    shouldQuery
      ? {
          TextoBuscado: textoBuscado,
          defaultDbAlias: "",
          tbl_control: campo.tbl_control ?? "",
          name_campo: "Destinatario_Cor",
          ...(campoIdScript !== undefined ? { idScript: campoIdScript } : {}),
          CDeRelacionEstadoRetriccionDto: relacionEstadoRestriccionDestinatario,
        }
      : null,
    shouldQuery,
  );

  useEffect(() => {
    setValue([]);
    setSearchText("");
    setClickAutocompleteActive(false);
    setOpenSelect(false);
  }, [selectedTramiteId]);

  const options = data.map((item, index) => ({
    value: item.idValue ?? `${item.texValue}-${index}`,
    label: item.texValue,
  }));

  const destinatarioLabel = campo.aleas_campo ?? "Destinatario";
  const destinatarioTitle = campo.title_control ?? "";
  const destinatarioTooltip = campo.tooltipAyuda ?? "";
  const destinatarioTooltipId = destinatarioTooltip
    ? "pl-radicacion-spe-tooltip-Destinatario_Cor"
    : undefined;
  const hasError = Boolean(error);

  const handleChange = (values: Array<{ value: string; label: React.ReactNode }>) => {
    const ultimo = values.slice(-1);
    setValue(ultimo);
    setOpenSelect(false);
    setClickAutocompleteActive(false);
    setSearchText("");
  };

  const tagRender = (props: {
    label: React.ReactNode;
    value: string;
    closable: boolean;
    onClose: () => void;
  }) => {
    const { label, value, closable, onClose } = props;
    return (
      <Tag closable={false}>
        {label}
        <Dropdown
          menu={{
            items: [
              {
                key: "info",
                label: (
                  <Space>
                    <InfoCircleOutlined />
                    Información
                  </Space>
                ),
                onClick: () => {
                  onOpenInfo({
                    id: Number(value) || 0,
                    nombre: String(label ?? ""),
                  });
                },
              },
            ],
          }}
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
    <Form.Item
      name="destinatario"
      label={
        <span title={destinatarioTitle}>
          {destinatarioLabel}
          {destinatarioTooltip ? (
            <Tooltip title={destinatarioTooltip}>
              <span
                className={`${styles["tooltip-ayuda"]} tooltip-ayuda`}
                role="button"
                tabIndex={0}
                aria-label={`Mostrar ayuda para ${destinatarioLabel}`}
                aria-describedby={destinatarioTooltipId}
                data-tooltip-id={destinatarioTooltipId}
              >
                <InfoCircleOutlined />
              </span>
            </Tooltip>
          ) : null}
        </span>
      }
      rules={
        campo.obligatorio_campo === 1
          ? [{ required: true, message: "Seleccione destinatario" }]
          : undefined
      }
      validateStatus={hasError ? "error" : undefined}
      help={
        hasError ? "No fue posible cargar las opciones. Intenta nuevamente." : undefined
      }
      data-ident="pl-radicacion-spe-Destinatario_Cor"
    >
      <Space.Compact style={{ width: "100%" }} id="destinatario">
        <Select
          mode="multiple"
          labelInValue
          value={value}
          showSearch
          searchValue={searchText}
          placeholder="Escriba para buscar destinatario"
          options={options}
          filterOption={false}
          tagRender={tagRender}
          open={openSelect}
          disabled={tagMenuOpen || campo.disable_campo === 1 || selectDisabledByRestriction}
          autoClearSearchValue={false}
          loading={isLoading || isFetching}
          maxCount={1}
          data-ident="pl-radicacion-spe-Destinatario_Cor"
          aria-label={destinatarioLabel}
          aria-describedby={destinatarioTooltipId}
          aria-required={campo.obligatorio_campo === 1}
          onSearch={(text) => {
            setSearchText(text);
            setClickAutocompleteActive(false);
            if (text.length > 0 && !tagMenuOpen) {
              setOpenSelect(true);
            } else {
              setOpenSelect(false);
            }
          }}
          onChange={handleChange}
          onOpenChange={(visible) => {
            if (!visible) {
              setOpenSelect(false);
              setClickAutocompleteActive(false);
              return;
            }
            if (
              canActivateAutocompleteOnClick &&
              hasSelectedTramite &&
              !tagMenuOpen
            ) {
              setClickAutocompleteActive(true);
              setOpenSelect(true);
            }
          }}
        />
      </Space.Compact>
    </Form.Item>
  );
};

/* =========================================================
   COMPONENTE PRINCIPAL
========================================================= */

interface FormRadicacionProps {
  plantilla: PlantillaRadicadoDTO;
  camposPlantilla: ReadonlyArray<CampoPlantillaDTO>;
}

const FormRadicacion: React.FC<FormRadicacionProps> = ({
  plantilla,
  camposPlantilla,
}) => {
  const [form] = Form.useForm();
  void plantilla;

  const isLoadingCamposPlantilla = false;

  const usuarios: Usuario[] = [
  ];

  const opcionesUsuarios = usuarios.map((user) => ({
    label: user.nombre,
    value: user.id,
  }));

  const [modalVisible, setModalVisible] = useState(false);
  const [usuarioSeleccionado, setUsuarioSeleccionado] =
    useState<Usuario | null>(null);
  const [selectedTramiteId, setSelectedTramiteId] = useState<string | null>(null);
  const [hasUserChangedTramite, setHasUserChangedTramite] = useState(false);

  const [resetKey, setResetKey] = useState(0);
  const { data: relacionEstadoRestriccionDestinatario } =
    useEstructuraRelacionTipoRestriccion(selectedTramiteId, hasUserChangedTramite);
  const { handleClearRadicacionForm } = useRadicacionFormReset<Usuario>({
    form,
    setSelectedTramiteId,
    setHasUserChangedTramite,
    setResetKey,
    setModalVisible,
    setUsuarioSeleccionado,
  });

  const abrirInformacion = (id: number) => {
    const user = usuarios.find((u) => u.id === id);
    if (user) {
      setUsuarioSeleccionado(user);
      setModalVisible(true);
    }
  };

  const camposPlantillaSafe = camposPlantilla;
  const normalizeCampoName = (value: string | null | undefined) =>
    String(value ?? "").trim().toUpperCase();

  const campoTramite = useMemo(
    () =>
      camposPlantillaSafe.find(
        (campo) => campo.name_campo === "Descripcion_Documento",
      ),
    [camposPlantillaSafe],
  );

  const tramiteOptions = useMemo(() => {
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
  const campoRemitenteCor = useMemo(
    () => {
      const campo = camposPlantillaSafe.find(
        (item) => normalizeCampoName(item.name_campo) === "REMITENTE_COR",
      );
      if (!campo) return undefined;
      return {
        ...campo,
        name_campo: "REMITENTE_COR",
      };
    },
    [camposPlantillaSafe],
  );
  const campoDestinatarioCor = useMemo(() => {
    const campo = camposPlantillaSafe.find(
      (item) => normalizeCampoName(item.name_campo) === "DESTINATARIO_COR",
    );
    if (!campo) return undefined;
    return {
      ...campo,
      name_campo: "Destinatario_Cor",
    };
  }, [camposPlantillaSafe]);

  const camposEspecializados = useMemo(
    () =>
      camposPlantillaSafe.filter(
        (campo) =>
          !["REMITENTE_COR", "DESTINATARIO_COR"].includes(
            normalizeCampoName(campo.name_campo),
          ),
      ),
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

  const {
    data: flujosRelacionados,
    error: flujosRelacionadosError,
    isLoading: isLoadingFlujosRelacionados,
  } = useFlujosRelacionadosTramite(selectedTramiteId, true);

  const flujoOptions = useMemo(
    () => (selectedTramiteId ? flujosRelacionados : []),
    [flujosRelacionados, selectedTramiteId],
  );

  useEffect(() => {
    if (!selectedTramiteId || flujosRelacionadosError || flujoOptions.length === 0) {
      form.setFieldValue("flujo", undefined);
    }
  }, [flujoOptions.length, flujosRelacionadosError, form, selectedTramiteId]);

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

  const destinatarioLabel =
    campoDestinatarioCor?.aleas_campo ?? "Destinatario";
  const destinatarioTitle = campoDestinatarioCor?.title_control ?? "";
  const destinatarioTooltip = campoDestinatarioCor?.tooltipAyuda ?? "";
  const destinatarioTooltipId = destinatarioTooltip
    ? "pl-radicacion-spe-tooltip-Destinatario_Cor"
    : undefined;
  const destinatarioRequired =
    campoDestinatarioCor?.obligatorio_campo === 1 || !campoDestinatarioCor;
  const destinatarioDisabledByRestriccion =
    relacionEstadoRestriccionDestinatario.IdTipoRestriccion > 0 &&
    relacionEstadoRestriccionDestinatario.ModuloRadicacionSimple === 0;
  const destinatarioDisabled =
    campoDestinatarioCor?.disable_campo === 1 || destinatarioDisabledByRestriccion;

  const destinatarioLabelNode = (
    <span title={destinatarioTitle}>
      {destinatarioLabel}
      {destinatarioTooltip ? (
        <Tooltip title={destinatarioTooltip}>
          <span
            className={`${styles["tooltip-ayuda"]} tooltip-ayuda`}
            role="button"
            tabIndex={0}
            aria-label={`Mostrar ayuda para ${destinatarioLabel}`}
            aria-describedby={destinatarioTooltipId}
            data-tooltip-id={destinatarioTooltipId}
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
                  key={`tipo-radicado-${resetKey}`}
                  label={tipoRadicadoLabelNode}
                  name="tipoRadicado"
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
                  <CampoPlantillaAutoCompleteField
                    key={`anexos-${resetKey}`}
                    campo={campoAnexos}
                  />
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
                    onChange={(value) => {
                      setHasUserChangedTramite(true);
                      const normalized = String(value ?? "").trim();
                      setSelectedTramiteId(normalized.length > 0 ? normalized : null);
                    }}
                  />
                </Form.Item>
              </Col>

              <Col xs={24} md={8}>
                <Form.Item label={flujoLabelNode} name="flujo">
                  <Select
                    placeholder="Seleccione"
                    options={flujoOptions}
                    data-ident="pl-radicacion-spe-RE_flujo_trabajo"
                    disabled={
                      campoFlujo?.disable_campo === 1 ||
                      !selectedTramiteId ||
                      isLoadingFlujosRelacionados
                    }
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
              <CampoPlantillaAutoCompleteField
                key={`asunto-${resetKey}`}
                campo={campoAsunto}
              />
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
            {campoRemitenteCor ? (
              <SelectRemitenteToken
                key={`remitente-token-${resetKey}`}
                campo={campoRemitenteCor}
                onOpenInfo={({ id, nombre }) => {
                  setUsuarioSeleccionado({ id, nombre });
                  setModalVisible(true);
                }}
              />
            ) : (
              <SelectRemitente
                rules={[{ required: true, message: "Seleccione remitente" }]}
                key={`remitente-${resetKey}`}
                label="Remitente"
                name="remitente"
                data-ident="pl-radicacion-spe-REMITENTE_COR"
                opciones={opcionesUsuarios}
                abrirInformacion={abrirInformacion}
              />
            )}
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
            {campoDestinatarioCor ? (
              <SelectDestinatarioToken
                key={`destinatario-token-${resetKey}`}
                campo={campoDestinatarioCor}
                selectDisabledByRestriction={destinatarioDisabledByRestriccion}
                relacionEstadoRestriccionDestinatario={relacionEstadoRestriccionDestinatario}
                selectedTramiteId={selectedTramiteId}
                onOpenInfo={({ id, nombre }) => {
                  setUsuarioSeleccionado({ id, nombre });
                  setModalVisible(true);
                }}
              />
            ) : (
              <SelectDestinatario
                rules={
                  destinatarioRequired
                    ? [{ required: true, message: "Seleccione destinatario" }]
                    : undefined
                }
                key={`destinatario-${resetKey}`}
                label={destinatarioLabelNode}
                name="destinatario"
                formItemDataIdent="pl-radicacion-spe-Destinatario_Cor"
                selectDataIdent="pl-radicacion-spe-Destinatario_Cor"
                ariaLabel={destinatarioLabel}
                selectDisabled={destinatarioDisabled}
                opciones={opcionesUsuarios}
                abrirInformacion={abrirInformacion}
              />
            )}
          </Card>

          {camposEspecializados.length > 0 ? (
            <CamposPlantillaAutoCompleteRenderer
              key={`campos-especializados-${resetKey}`}
              camposPlantilla={camposEspecializados}
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
        >
          Documentos IA
        </Button>

        {/* DERECHA */}
        <div className={styles.rightGroup}>
          <Button
            icon={<DeleteFilled />}
            className={styles.btnClear}
            onClick={handleClearRadicacionForm}
          >
            Limpiar
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
