import { SaveFilled } from "@ant-design/icons";
import { AppButton } from "../../AppButton";
import type { AppEditorSaveStatus } from "../domain/save-state.types";
import styles from "./AppEditorSaveAction.module.css";

type AppEditorSaveActionProps = {
  disabled?: boolean;
  iconOnly?: boolean;
  onSave: () => void;
  saveStatus: AppEditorSaveStatus;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

export function AppEditorSaveAction({
  disabled = false,
  iconOnly = false,
  onSave,
  saveStatus,
}: AppEditorSaveActionProps) {
  const isDirty = saveStatus === "dirty";

  return (
    <AppButton
      variant="secondary"
      size="sm"
      disabled={disabled}
      onClick={onSave}
      icon={iconOnly ? <SaveFilled /> : undefined}
      aria-label="Guardar"
      tooltip={iconOnly ? "Guardar" : undefined}
      className={joinClasses(
        isDirty ? styles.saveActionDirty : styles.saveActionIdle,
      )}
    >
      {iconOnly ? undefined : "Guardar"}
    </AppButton>
  );
}
