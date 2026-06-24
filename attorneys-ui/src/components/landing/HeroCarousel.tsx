import { useEffect, useState } from "react";
import { Box, Container, Typography } from "@mui/material";
import { publicMediaUrl } from "../../utils/mediaUrl";

type Slide = { id: number; imageUrl: string; caption?: string };

type HeroCarouselProps = {
  slides: Slide[];
  overlay?: React.ReactNode;
};

const HeroCarousel = ({ slides, overlay }: HeroCarouselProps) => {
  const [index, setIndex] = useState(0);
  const hasSlides = slides.length > 0;

  useEffect(() => {
    if (slides.length <= 1) return;
    const timer = setInterval(() => setIndex((i) => (i + 1) % slides.length), 5000);
    return () => clearInterval(timer);
  }, [slides.length]);

  return (
    <Box
      component="section"
      sx={{
        position: "relative",
        width: "100%",
        minHeight: hasSlides
          ? { xs: 440, sm: 560, md: 640, lg: 720 }
          : { xs: 320, sm: 420 },
        overflow: "hidden",
        color: "#fff",
        background: "linear-gradient(135deg, #0d1b2a 0%, #1b263b 50%, #415a77 100%)",
      }}
    >
      {hasSlides &&
        slides.map((slide, i) => (
          <Box
            key={slide.id}
            aria-hidden={i !== index}
            sx={{
              position: "absolute",
              inset: 0,
              opacity: i === index ? 1 : 0,
              transition: "opacity 1s ease-in-out",
              backgroundImage: `url(${publicMediaUrl(slide.imageUrl)})`,
              backgroundSize: "cover",
              backgroundPosition: { xs: "center top", md: "center 15%" },
            }}
          />
        ))}

      <Box
        sx={{
          position: "absolute",
          inset: 0,
          background: hasSlides
            ? "linear-gradient(90deg, rgba(13,27,42,0.92) 0%, rgba(13,27,42,0.65) 55%, rgba(13,27,42,0.35) 100%)"
            : "transparent",
        }}
      />

      <Box sx={{ position: "relative", zIndex: 1, py: { xs: 5, sm: 8 } }}>
        {overlay && (
          <Container maxWidth={false} sx={{ px: { xs: 2, sm: 3, md: 4 } }}>
            {overlay}
          </Container>
        )}
      </Box>

      {hasSlides && slides.length > 1 && (
        <Box
          sx={{
            position: "absolute",
            bottom: 16,
            left: "50%",
            transform: "translateX(-50%)",
            display: "flex",
            gap: 1,
            zIndex: 2,
          }}
        >
          {slides.map((slide, i) => (
            <Box
              key={slide.id}
              onClick={() => setIndex(i)}
              sx={{
                width: 10,
                height: 10,
                borderRadius: "50%",
                bgcolor: i === index ? "#c9a227" : "rgba(255,255,255,0.4)",
                cursor: "pointer",
              }}
            />
          ))}
        </Box>
      )}

      {hasSlides && slides[index]?.caption && (
        <Typography
          variant="caption"
          sx={{
            position: "absolute",
            bottom: 16,
            right: 24,
            zIndex: 2,
            color: "rgba(255,255,255,0.85)",
          }}
        >
          {slides[index].caption}
        </Typography>
      )}
    </Box>
  );
};

export default HeroCarousel;
