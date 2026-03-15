import { describe, expect, test } from "bun:test";
import { dummyCourses } from "./dummyCourses";

describe("dummyCourses", () => {
  test("exports a non-empty array of courses", () => {
    expect(dummyCourses.length).toBeGreaterThan(0);
  });

  test("each course has required properties", () => {
    for (const course of dummyCourses) {
      expect(course.id).toBeTruthy();
      expect(course.name).toBeTruthy();
      expect(Array.isArray(course.holes)).toBe(true);
      expect(course.holes.length).toBeGreaterThan(0);
    }
  });

  test("totalPar matches the sum of hole pars", () => {
    for (const course of dummyCourses) {
      const sum = course.holes.reduce((acc, h) => acc + h.par, 0);
      expect(course.totalPar).toBe(sum);
    }
  });

  test("totalLength matches the sum of hole lengths", () => {
    for (const course of dummyCourses) {
      const sum = course.holes.reduce((acc, h) => acc + h.length, 0);
      expect(course.totalLength).toBe(sum);
    }
  });

  test("each hole has a positive par and length", () => {
    for (const course of dummyCourses) {
      for (const hole of course.holes) {
        expect(hole.par).toBeGreaterThan(0);
        expect(hole.length).toBeGreaterThan(0);
        expect(hole.number).toBeGreaterThan(0);
      }
    }
  });
});
