import { SearchOutlined } from "@ant-design/icons";
import { forwardRef } from "react";
import type { InputRef } from "antd";
import { AppInput } from "../AppInput";
import type { AppInputTextProps } from "../AppInput";
import styles from "./AppInputSearch.module.css";

export type AppInputSearchProps = Omit<AppInputTextProps, "prefix" | "type"> & {
  showIcon?: boolean;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

export const AppInputSearch = forwardRef<InputRef, AppInputSearchProps>(
  function AppInputSearch({ className, showIcon = true, ...props }, ref) {
    const searchIcon = showIcon ? (
      <span className={styles.icon} aria-hidden="true">
        <SearchOutlined />
      </span>
    ) : undefined;

    return (
      <AppInput
        {...props}
        ref={ref}
        prefix={searchIcon}
        className={joinClasses(styles.input, className)}
      />
    );
  },
);
