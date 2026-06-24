import { Box } from "@mui/material";
import type { ElementType, ReactNode } from "react";

type PageWidthProps = {
  children: ReactNode;
  id?: string;
  className?: string;
  sx?: object;
};

/** Centered max-width content column. */
export const PageWidth = ({ children, id, className, sx }: PageWidthProps) => (
  <Box
    id={id}
    className={className ? `layout-container ${className}` : "layout-container"}
    sx={{
      width: "100%",
      maxWidth: 1200,
      mx: "auto",
      boxSizing: "border-box",
      ...sx,
    }}
  >
    {children}
  </Box>
);

type FullBleedSectionProps = {
  children: ReactNode;
  id?: string;
  component?: ElementType;
  className?: string;
  sx?: object;
};

/** Full-width section background with centered inner content. */
export const FullBleedSection = ({
  children,
  id,
  component = "section",
  className,
  sx,
}: FullBleedSectionProps) => (
  <Box
    id={id}
    component={component}
    className={className}
    sx={{ width: "100%", boxSizing: "border-box", ...sx }}
  >
    <Box className="layout-container" sx={{ width: "100%", maxWidth: 1200, mx: "auto", boxSizing: "border-box" }}>
      {children}
    </Box>
  </Box>
);

/** @deprecated Use PageWidth */
export const SiteContainer = PageWidth;

/** @deprecated Use FullBleedSection */
export const SiteSection = FullBleedSection;
