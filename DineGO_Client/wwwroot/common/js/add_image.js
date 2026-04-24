function setupImagePreviewByPrefix(prefix) {
  const input = document.getElementById(`${prefix}_input`);
  const addBtn = document.getElementById(`${prefix}_addBtn`);
  const previewBox = document.getElementById(`${prefix}_previewBox`);
  const previewImg = document.getElementById(`${prefix}_previewImg`);
  const removeBtn = document.getElementById(`${prefix}_removeBtn`);

  if (!input || !addBtn || !previewBox || !previewImg || !removeBtn) return;

  addBtn.addEventListener("click", () => input.click());

  input.addEventListener("change", function () {
    if (this.files && this.files[0]) {
      const reader = new FileReader();
      reader.onload = function (e) {
        previewImg.src = e.target.result;
        previewBox.style.display = "block";
        addBtn.style.display = "none";
      };
      reader.readAsDataURL(this.files[0]);
    }
  });

  removeBtn.addEventListener("click", function () {
    previewImg.src = "";
    input.value = "";
    previewBox.style.display = "none";
    addBtn.style.display = "block";
  });
}

function setupMultipleImagePreviewGrid(prefix) {
  const input = document.getElementById(`${prefix}_input`);
  const addBtn = document.getElementById(`${prefix}_addBtn`);
  const previewBox = document.getElementById(`${prefix}_previewBox`);
  if (!input || !addBtn || !previewBox) return;

  addBtn.addEventListener("click", () => input.click());

  input.addEventListener("change", function () {
    const newFiles = Array.from(this.files);
    if (newFiles.length === 0) return;

    const currentImages = previewBox.querySelectorAll(".image-full, .image-half, .image-grid");

    const allFiles = [];

    // ✅ Lấy ảnh đang hiển thị từ preview (nếu có), convert về File
    currentImages.forEach(imgDiv => {
      const img = imgDiv.querySelector("img");
      if (img?.src?.startsWith("data:image")) {
        allFiles.push(dataURLtoFile(img.src, "image.jpg"));
        imgDiv.remove();
      }
    });

    // ✅ Thêm ảnh mới từ input
    allFiles.push(...newFiles);


    // ✅ Render lại ảnh theo tổng số lượng
    renderByCount(allFiles);
    // ✅ gọi update để ẩn video nếu có ảnh
    updateUploadVisibility();

    // ❌ KHÔNG reset input.value để giữ lại files cho form submit
  });

  function renderByCount(files) {
    if (files.length === 1) {
      renderSingleImage(files[0]);
    } else if (files.length === 2) {
      renderTwoImages(files);
    } else {
      renderAllImages(files);
    }
  }

  function renderSingleImage(file) {
    const reader = new FileReader();
    reader.onload = function (e) {
      const div = document.createElement("div");
      div.className = "image-full";
      div.innerHTML = `
        <img src="${e.target.result}" />
        <button class="remove-btn">×</button>
      `;
      previewBox.insertBefore(div, addBtn); // thêm trước nút
      div.querySelector(".remove-btn").onclick = () => handleRemove(div);
    };
    reader.readAsDataURL(file);
  }

  function renderTwoImages(files) {
    files.forEach(file => {
      const reader = new FileReader();
      reader.onload = function (e) {
        const div = document.createElement("div");
        div.className = "image-half";
        div.innerHTML = `
          <img src="${e.target.result}" />
          <button class="remove-btn">×</button>
        `;
        previewBox.insertBefore(div, addBtn);
        div.querySelector(".remove-btn").onclick = () => handleRemove(div);
      };
      reader.readAsDataURL(file);
    });
  }

  function renderAllImages(files) {
    files.forEach(file => {
      const reader = new FileReader();
      reader.onload = function (e) {
        const div = document.createElement("div");
        div.className = "image-grid";
        div.innerHTML = `
          <img src="${e.target.result}" />
          <button class="remove-btn">×</button>
        `;
        previewBox.insertBefore(div, addBtn);
        div.querySelector(".remove-btn").onclick = () => handleRemove(div);
      };
      reader.readAsDataURL(file);
    });
  }

  function handleRemove(div) {
    div.remove();

    const remaining = previewBox.querySelectorAll(".image-full, .image-half, .image-grid");
    if (remaining.length === 0) {
      input.value = ""; // reset input
    }

    // render lại ảnh còn lại (nếu có)
    const files = [];
    remaining.forEach(imgDiv => {
      const img = imgDiv.querySelector("img");
      if (img?.src?.startsWith("data:image")) {
        files.push(dataURLtoFile(img.src, "image.jpg"));
        imgDiv.remove();
      }
    });
    if (files.length > 0) {
      renderByCount(files);
    }

    // ✅ cập nhật hiển thị
    updateUploadVisibility();
  }


  function dataURLtoFile(dataurl, filename) {
    const arr = dataurl.split(',');
    const mime = arr[0].match(/:(.*?);/)[1];
    const bstr = atob(arr[1]);
    let n = bstr.length;
    const u8arr = new Uint8Array(n);
    while (n--) {
      u8arr[n] = bstr.charCodeAt(n);
    }
    return new File([u8arr], filename, { type: mime });
  }
}

function setupVideoPreview(prefix) {
  const input = document.getElementById(`${prefix}_input`);
  const addBtn = document.getElementById(`${prefix}_addBtn`);
  const previewBox = document.getElementById(`${prefix}_previewBox`);

  if (!input || !addBtn || !previewBox) return;

  addBtn.addEventListener("click", () => input.click());

  input.addEventListener("change", function () {
    if (this.files && this.files[0]) {
      const file = this.files[0];
      const url = URL.createObjectURL(file);

      // Clear preview cũ (nếu chỉ cho 1 video)
      previewBox.querySelectorAll(".video-preview").forEach(v => v.remove());

      const div = document.createElement("div");
      div.className = "video-preview";
      div.innerHTML = `
        <video src="${url}" controls style="max-width: 100%; border-radius: 8px;"></video>
        <button type="button" class="remove-btn">×</button>
      `;
      previewBox.insertBefore(div, addBtn);

      div.querySelector(".remove-btn").onclick = () => {
        div.remove();
        input.value = "";
        updateUploadVisibility(); // ✅ gọi lại
      };
      updateUploadVisibility();  //gọi ngay khi chọn video
    }
  });
}

function updateUploadVisibility() {
  const imageInput = document.getElementById("post_multi_input");
  const videoInput = document.getElementById("post_video_input");
  const imageBox = document.getElementById("post_multi_previewBox");
  const videoBox = document.getElementById("post_video_previewBox");

  if (imageInput.files.length > 0) {
    videoBox.style.display = "none";
  } else if (videoInput.files.length > 0) {
    imageBox.style.display = "none";
  } else {
    // ✅ Không có ảnh cũng không có video → hiện cả 2
    imageBox.style.display = "block";
    videoBox.style.display = "block";
  }
}
