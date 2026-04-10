import type { AppTabItem } from "../../../app/Components/UI/AppTabs";
import { AppTabs } from "../../../app/Components/UI/AppTabs";

export default function GestionRespuesta() {
  const items: AppTabItem[] = [
    {
      key: "contexto",
      label: "Contexto",
      children: null,
    },
    {
      key: "detalle",
      label: "Detalle",
      children: null,
    },
  ];

  return (
    <AppTabs items={items} fullWidth />
  );
}
