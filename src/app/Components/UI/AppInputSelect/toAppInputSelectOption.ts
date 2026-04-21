import type {
  AppInputSelectBackendItem,
  AppInputSelectOption,
} from "./AppInputSelect";

type Primitive = string | number;

export const toAppInputSelectOption = <TValue extends Primitive = Primitive>(
  item: AppInputSelectBackendItem,
) =>
  ({
    label: item.nombre,
    value: item.id as TValue,
    disabled: item.activo === false,
  }) satisfies AppInputSelectOption<TValue>;

