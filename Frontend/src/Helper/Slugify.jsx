const slugify = (str) => {
  if (typeof str !== "string") return ""; // hoặc trả về null, hoặc throw error, tùy ý bạn

  return str
    .toLowerCase()
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/đ/g, "d")
    .replace(/Đ/g, "D")
    .replace(/\s+/g, "-");
};

export default slugify
