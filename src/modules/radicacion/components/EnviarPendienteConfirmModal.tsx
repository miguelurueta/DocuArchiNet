import { Button, Modal, message } from "antd";
import { RocketFilled } from "@ant-design/icons";
import { useState } from "react";
import { useEnviarRadicadoPendiente } from "../hooks/useEnviarRadicadoPendiente";

export function EnviarPendienteConfirmModal() {
  const [open, setOpen] = useState(false);
  const { enviarActivoAPendiente, puedeEnviarAPendiente, isSending } =
    useEnviarRadicadoPendiente({
      onSuccess: (successMessage) => {
        message.success(successMessage);
        setOpen(false);
      },
      onError: (errorMessage) => {
        message.error(errorMessage);
      },
    });

  if (!puedeEnviarAPendiente) {
    return null;
  }

  return (
    <>
      <Button
        icon={<RocketFilled />}
        loading={isSending}
        disabled={isSending}
        onClick={() => setOpen(true)}
      >
        Enviar a pendiente
      </Button>

      <Modal
        title="Enviar tramite a pendiente"
        open={open}
        okText="Confirmar"
        cancelText="Cancelar"
        confirmLoading={isSending}
        okButtonProps={{ disabled: isSending }}
        cancelButtonProps={{ disabled: isSending }}
        closable={!isSending}
        maskClosable={!isSending}
        onOk={enviarActivoAPendiente}
        onCancel={() => {
          if (!isSending) {
            setOpen(false);
          }
        }}
      >
        <p>
          El tramite activo se devolvera a pendientes y el panel de documentos
          quedara deshabilitado hasta que vuelva a ser asignado.
        </p>
      </Modal>
    </>
  );
}
