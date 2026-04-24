document.addEventListener("DOMContentLoaded", function () {
  // --- Toggle dropdown (delete post) ---
  document.querySelectorAll(".toggle-dropdown").forEach(function (btn) {
    btn.addEventListener("click", function (e) {
      const menu = btn.nextElementSibling;
      document.querySelectorAll(".custom-dropdown-menu").forEach(m => {
        if (m !== menu) m.classList.add("d-none");
      });
      menu.classList.toggle("d-none");
      e.stopPropagation();
    });
  });

  document.addEventListener("click", function () {
    document.querySelectorAll(".custom-dropdown-menu").forEach(m => m.classList.add("d-none"));
  });

  // 👈 THÊM MỚI: Function update hiển thị nút add media (ảnh/video) theo BR
  function updateMediaButtons() {
    const previewBox = document.getElementById("edit_multi_previewBox");
    const videoPreview = document.getElementById("editVideoPreview");
    const addImageBtn = document.getElementById("edit_multi_addBtn"); // Nút add ảnh multiple
    const addVideoBtn = document.getElementById("editAddVideoBtn"); // Nút add video

    if (!previewBox || !videoPreview || !addImageBtn || !addVideoBtn) return;

    const hasImages = previewBox.querySelectorAll(".image-full, .image-half, .image-grid").length > 0;
    const hasVideo = videoPreview.style.display === "block";

    if (hasImages && hasVideo) {
      // Không cho phép cả hai: Ưu tiên giữ ảnh, clear video (adjust nếu cần ưu tiên video)
      const previewVideo = document.getElementById("editPreviewVideo");
      const editPostVideoInput = document.getElementById("edit_post_video");
      const oldVideoHidden = document.getElementById("edit_old_video");
      previewVideo.querySelector("source").src = "";
      previewVideo.load();
      editPostVideoInput.value = "";
      oldVideoHidden.value = "";
      videoPreview.style.display = "none";
      hasVideo = false; // Update lại
    }

    if (hasImages) {
      // Có ảnh: Ẩn add video, hiện add ảnh (nếu chưa full, ví dụ <10 ảnh)
      addVideoBtn.style.display = "none";
      const imageCount = previewBox.querySelectorAll(".image-full, .image-half, .image-grid").length;
      addImageBtn.style.display = (imageCount < 10) ? "" : "none"; // Giả sử max 10 ảnh
    } else if (hasVideo) {
      // Có video: Ẩn add ảnh và add video (video single)
      addImageBtn.style.display = "none";
      addVideoBtn.style.display = "";
    } else {
      // Không có gì: Hiện cả hai
      addImageBtn.style.display = "";
      addVideoBtn.style.display = "";
    }
  }

  // --- Trigger mở modal ---
  document.addEventListener("click", function (e) {
    // --- Trigger mở modal tạo bài viết ---
    if (e.target.classList.contains("trigger-create-post-popup")) {
      const modal = new bootstrap.Modal(document.getElementById("createPostModal"));
      modal.show();
      bindEmojiPicker(document.getElementById("createPostModal"));
    }
    // --- Trigger mở modal chỉnh sửa bài viết ---
    const triggerEdit = e.target.closest(".trigger-edit-post-popup");
    if (triggerEdit) {
      const postId = triggerEdit.getAttribute("data-post-id");
      const content = triggerEdit.getAttribute("data-post-content");
      const imageJson = triggerEdit.getAttribute("data-post-image");
      const videoFile = triggerEdit.getAttribute("data-post-video");

      document.getElementById("edit_post_id").value = postId;
      document.getElementById("edit_post_content").value = content;

      const previewBox = document.getElementById("edit_multi_previewBox");
      const addBtn = document.getElementById("edit_multi_addBtn");

      // Xóa preview cũ
      previewBox.querySelectorAll(".image-full, .image-half, .image-grid, .video-preview")
        .forEach(el => el.remove());

      // Load ảnh cũ
      try {
        const imageList = JSON.parse(imageJson) || [];
        const count = imageList.length;
        imageList.slice(0, 10).forEach(imgName => {
          const div = document.createElement("div");
          if (count === 1) div.className = "image-full";
          else if (count === 2) div.className = "image-half";
          else div.className = "image-grid";

          div.innerHTML = `
            <img src="https://dinego-bucket-aws.s3.ap-south-1.amazonaws.com/posts/thumb_${imgName}" data-name="${imgName}" />
            <button class="remove-btn">×</button>
          `;
          // 👈 CẬP NHẬT: Bind remove với update buttons
          div.querySelector(".remove-btn").onclick = () => {
            div.remove();
            updateMediaButtons(); // Gọi update sau khi xóa
          };
          previewBox.insertBefore(div, addBtn);
        });
      } catch (err) {
        console.error("Không parse được ảnh edit:", err);
      }

      // Load video cũ (nếu có)
      const videoPreview = document.getElementById("editVideoPreview");
      const previewVideo = document.getElementById("editPreviewVideo");
      const removeVideoBtn = document.getElementById("editRemoveVideoBtn");

      if (videoFile) {
        previewVideo.querySelector("source").src =
          `https://dinego-bucket-aws.s3.ap-south-1.amazonaws.com/posts/videos/${videoFile}`;
        previewVideo.load();
        videoPreview.style.display = "block";

        // gán tên video để submit
        document.getElementById("edit_old_video").value = videoFile;
      } else {
        videoPreview.style.display = "none";
      }

      // 👈 CẬP NHẬT: Bind nút xóa video với update buttons
      if (removeVideoBtn) {
        removeVideoBtn.onclick = function () {
          previewVideo.querySelector("source").src = "";
          previewVideo.load();
          document.getElementById("edit_post_video").value = "";
          document.getElementById("edit_old_video").value = "";
          videoPreview.style.display = "none";
          updateMediaButtons(); // Gọi update sau khi xóa
        };
      }

      // Show modal
      const modal = new bootstrap.Modal(document.getElementById("editPostModal"));
      modal.show();

      modal._element.addEventListener("shown.bs.modal", function () {
        setupMultipleImagePreviewGrid("edit_multi");  // Bind cho ảnh multiple
        bindEmojiPicker(document.getElementById("editPostModal"));  // Emoji

        // 👈 THÊM MỚI: Bind nút thêm video và preview (với update buttons)
        const editAddVideoBtn = document.getElementById("editAddVideoBtn");
        const editPostVideoInput = document.getElementById("edit_post_video");
        const editVideoPreview = document.getElementById("editVideoPreview");
        const editPreviewVideo = document.getElementById("editPreviewVideo");
        const editRemoveVideoBtn = document.getElementById("editRemoveVideoBtn");

        if (editAddVideoBtn && editPostVideoInput && editVideoPreview) {
          // Click nút "Thêm video" → mở file picker
          editAddVideoBtn.addEventListener("click", function (e) {
            e.preventDefault();
            editPostVideoInput.click();
          });

          // Khi chọn file video mới → preview video và update buttons
          editPostVideoInput.addEventListener("change", function () {
            if (this.files && this.files[0]) {
              const file = this.files[0];
              if (file.type.startsWith('video/') && file.size < 100 * 1024 * 1024) {
                const reader = new FileReader();
                reader.onload = function (e) {
                  editPreviewVideo.innerHTML = '';
                  const source = document.createElement('source');
                  source.src = e.target.result;
                  source.type = file.type;
                  editPreviewVideo.appendChild(source);
                  editPreviewVideo.load();

                  editVideoPreview.style.display = "block";

                  // Clear old video
                  document.getElementById("edit_old_video").value = "";
                  
                  updateMediaButtons(); // 👈 Gọi update sau khi add video
                };
                reader.readAsDataURL(file);
              } else {
                alert("Vui lòng chọn file video hợp lệ (dưới 100MB)!");
                this.value = "";
              }
            }
          });

          // Bind nút xóa video (đã có ở trên, nhưng đảm bảo)
          if (editRemoveVideoBtn) {
            editRemoveVideoBtn.onclick = function (e) {
              e.preventDefault();
              editPreviewVideo.innerHTML = `
                <source src="" type="video/mp4">
                Trình duyệt của bạn không hỗ trợ video.
              `;
              editPreviewVideo.load();
              editPostVideoInput.value = "";
              document.getElementById("edit_old_video").value = "";
              editVideoPreview.style.display = "none";
              updateMediaButtons(); // Đảm bảo gọi update
            };
          }
        }

        // 👈 THÊM MỚI: Gọi update sau khi load tất cả (dựa trên dữ liệu cũ)
        setTimeout(() => updateMediaButtons(), 100); // Delay nhỏ để DOM ready
      });
    }
  });

  // --- Tạo bài viết ---
  const createForm = document.getElementById("createPostForm");
  if (createForm) {
    createForm.addEventListener("submit", function (e) {
      showGlobalLoader();
    });
  }

  // --- Chỉnh sửa bài viết (ảnh + video) ---
  document.getElementById("editPostForm").addEventListener("submit", function (e) {
    // Xử lý ảnh cũ
    const images = [];
    document.querySelectorAll("#edit_multi_previewBox img").forEach(img => {
      const name = img.dataset.name;
      if (name) images.push(name);
    });
    document.getElementById("edit_old_images").value = images.join(",");

    // 👈 CẬP NHẬT: Validate không cho submit cả ảnh + video
    const hasImages = images.length > 0;
    const videoInput = document.getElementById("edit_post_video");
    const oldVideoHidden = document.getElementById("edit_old_video");
    const hasVideo = (videoInput.files && videoInput.files.length > 0) || oldVideoHidden.value;

    if (hasImages && hasVideo) {
      e.preventDefault();
      showNotification("Không thể đăng cả ảnh và video cùng lúc! Vui lòng xóa một loại.", "danger");
      return;
    }

    // Xử lý video (giữ nguyên validate size)
    if (videoInput && oldVideoHidden) {
      if (videoInput.files && videoInput.files.length > 0) {
        const file = videoInput.files[0];
        if (!file.type.startsWith('video/') || file.size > 100 * 1024 * 1024) {
          e.preventDefault();
          showNotification("Video vượt quá 100MB.", "danger");
          return;
        }
        oldVideoHidden.value = "";
      }
    }

    console.log("🚀 Submit edit form");
    showGlobalLoader();
  });
});

// --- Xem chi tiết bài viết ---
function openDetailPostPopup(postId) {
  fetch(`/Post/Details?postId=${postId}`)
    .then(res => res.text())
    .then(html => {
      document.getElementById("detailPostPopupBody").innerHTML = html;
      const popup = new bootstrap.Modal(document.getElementById('detailPostModal'));
      popup.show();
      // Gắn lại logic comment
      bindCommentFormInDetailPopup();
      rebindEditDeleteHandlersInDetail();
      bindDynamicModalInDetail();
      bindEmojiPicker(document.getElementById("detailPostModal"));
    });
}

  // // 👈 THÊM MỚI: Nếu chưa có setupMultipleImagePreviewGrid, đây là ví dụ bind cho add multiple ảnh
  // // (Nếu bạn đã có function này, modify để gọi updateMediaButtons() sau khi append div mới)
  // function setupMultipleImagePreviewGrid(prefix) {
  //   const inputFile = document.getElementById(`${prefix}_post_image`); // e.g., edit_post_image
  //   const addBtn = document.getElementById(`${prefix}_multi_addBtn`); // e.g., edit_multi_addBtn
  //   const previewBox = document.getElementById(`${prefix}_multi_previewBox`);

  //   if (!inputFile || !addBtn || !previewBox) return;

  //   addBtn.addEventListener("click", function (e) {
  //     e.preventDefault();
  //     inputFile.click();
  //   });

  //   inputFile.addEventListener("change", function () {
  //     if (this.files && this.files.length > 0) {
  //       const currentCount = previewBox.querySelectorAll(".image-full, .image-half, .image-grid").length;
  //       if (currentCount >= 10) {
  //         alert("Tối đa 10 ảnh!");
  //         return;
  //       }

  //       Array.from(this.files).forEach((file, index) => {
  //         if (currentCount + index + 1 > 10) return; // Giới hạn

  //         if (file.type.startsWith('image/')) {
  //           const reader = new FileReader();
  //           reader.onload = function (e) {
  //             const div = document.createElement("div");
  //             const totalCount = currentCount + previewBox.querySelectorAll(".image-full, .image-half, .image-grid").length + 1;
  //             if (totalCount === 1) div.className = "image-full";
  //             else if (totalCount === 2) div.className = "image-half";
  //             else div.className = "image-grid";

  //             div.innerHTML = `
  //               <img src="${e.target.result}" alt="Preview" />
  //               <button class="remove-btn">×</button>
  //             `;
  //             div.querySelector(".remove-btn").onclick = () => {
  //               div.remove();
  //               updateMediaButtons(); // Update sau xóa
  //             };
  //             previewBox.insertBefore(div, addBtn);

  //             updateMediaButtons(); // 👈 Gọi update sau khi add ảnh
  //           };
  //           reader.readAsDataURL(file);
  //         }
  //       });
  //       this.value = ""; // Reset input sau add
  //     }
  //   });
  // }


// --- Xử lý Like ---
function showReactions(postId, btn) {
  const wrapper = btn.closest(".reaction-wrapper");
  const menu = wrapper.querySelector(`#reaction-menu-${postId}`);
  if (menu) {
    menu.style.display = "block";
  }
}

function hideReactions(wrapperEl) {
  const menu = wrapperEl.querySelector(".reaction-menu");
  if (menu) {
    menu.style.display = "none";
  }
}

function bindCommentFormInDetailPopup() {
  const form = document.querySelector('#detailPostModal .comment-form');
  if (!form) return;

  form.addEventListener('submit', async function (e) {
    e.preventDefault();

    const postId = form.querySelector('input[name="post_id"]').value;
    const cusId = form.querySelector('input[name="cus_id"]').value;
    const contentInput = form.querySelector('input[name="comment_content"]');
    const content = contentInput.value;

    if (!content.trim()) return;

    const response = await fetch('/Post/AddCommentAjax', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        post_id: postId,
        cus_id: cusId,
        comment_content: content
      })
    });

    if (response.ok) {
      const json = await response.json();
      if (json.message) {
        showNotification(json.message, "success");
      }

      const newDetailHtml = await fetch(`/Post/Details?postId=${postId}`).then(r => r.text());
      const parser = new DOMParser();
      const newDoc = parser.parseFromString(newDetailHtml, "text/html");

      const newCommentArea = newDoc.querySelector(".overflow-auto");
      const newForm = newDoc.querySelector(".comment-form");

      const detailModal = document.getElementById("detailPostModal");
      const modalCommentArea = detailModal.querySelector(".overflow-auto");
      const modalCommentForm = detailModal.querySelector(".comment-form");

      if (newCommentArea && modalCommentArea) {
        modalCommentArea.innerHTML = newCommentArea.innerHTML;
      }
      if (newForm && modalCommentForm) {
        modalCommentForm.querySelector('input[name="comment_content"]').value = '';
      }

      rebindEditDeleteHandlersInDetail();
      bindDynamicModalInDetail();

    } else {
      console.error("❌ Lỗi gửi bình luận:", await response.text());
    }
    //Cập nhật lượt comment mới khi add thành công
    const countSpan = document.querySelector(`#comment-count-${postId}`);
    if (countSpan) {
      // Lấy tổng số comment đang hiển thị
      const commentCount = document.querySelectorAll(`#detailPostModal .comment-content-area`).length;
      countSpan.textContent = `(${commentCount})`;
    }

  });
  rebindEditDeleteHandlersInDetail();
  bindDynamicModalInDetail();
}

function submitReactionAjax(emotionType, postId, label, button) {
  const wrapper = button.closest(".reaction-wrapper");
  const mainBtn = wrapper.querySelector(`#main-btn-${postId}`);

  const currentType = parseInt(mainBtn?.dataset.currentType || "0");
  const actualType = currentType === emotionType ? 0 : emotionType;

  fetch("/Post/ReactToPost", {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify({
      post_id: postId,
      like_emotion_type: actualType
    })
  })
    .then(async res => {
      const contentType = res.headers.get("Content-Type") || "";
      if (!res.ok) {
        const errorText = await res.text();
        throw new Error(`Lỗi ${res.status}: ${errorText}`);
      }
      if (!contentType.includes("application/json")) {
        const raw = await res.text();
        throw new Error(`Phản hồi không phải JSON: ${raw}`);
      }
      return res.json();
    })
    .then(data => {
      if (!mainBtn) return;

      // ✅ Cập nhật nút cảm xúc
      if (actualType === 0) {
        mainBtn.innerText = "👍 Thích";
        mainBtn.className = "main-reaction-btn btn-reaction default";
        mainBtn.dataset.currentType = "0";
      } else {
        mainBtn.innerText = label;
        mainBtn.className = "main-reaction-btn " + getReactionClass(actualType);
        mainBtn.dataset.currentType = actualType.toString();
      }

      // ✅ Gán lại khi đang trong modal chi tiết
      if (document.getElementById("detailPostModal")?.classList.contains("show")) {
        window.lastReactionUpdate = {
          postId: postId,
          label: label,
          type: actualType
        };
      }

      // ✅ Cập nhật số lượt like
      const countSpan = wrapper.parentElement.querySelector(".reaction-count");
      if (countSpan && data.newLikeCount !== undefined) {
        countSpan.innerText = `(${data.newLikeCount})`;
      }
    })
    .catch(err => {
      alert("Không thể gửi cảm xúc.");
      console.error("❌ Lỗi khi gửi cảm xúc:", err);
    });
}


// Trả về class theo emotion
function getReactionClass(type) {
  switch (type) {
    case 1: return "btn-reaction like";
    case 2: return "btn-reaction love";
    case 3: return "btn-reaction haha";
    case 4: return "btn-reaction wow";
    case 5: return "btn-reaction sad";
    case 6: return "btn-reaction angry";
    default: return "btn-reaction default";
  }
}

//Đóng details cập nhật trạng thái like
document.addEventListener("DOMContentLoaded", function () {
  const detailModal = document.getElementById("detailPostModal");
  if (detailModal) {
    detailModal.addEventListener("hidden.bs.modal", function () {
      if (window.lastReactionUpdate) {
        const { postId, label, type } = window.lastReactionUpdate;
        const indexBtn = document.querySelector(`#main-btn-${postId}`);
        if (indexBtn) {
          indexBtn.innerText = type === 0 ? "👍 Thích" : label;
          indexBtn.className = "main-reaction-btn " + getReactionClass(type);
          indexBtn.dataset.currentType = type.toString();
        }
        window.lastReactionUpdate = null;
      }
    });
  }
});

//Edit, delete comment _ Thang
function rebindEditDeleteHandlersInDetail() {
  const modal = document.querySelector('#detailPostModal');

  // Toggle dropdown ⋯
  modal.querySelectorAll('.toggle-dropdown').forEach(btn => {
    btn.onclick = function (e) {
      const menu = this.nextElementSibling;
      modal.querySelectorAll('.custom-dropdown-menu').forEach(m => {
        if (m !== menu) m.classList.add('d-none');
      });
      menu.classList.toggle('d-none');
      e.stopPropagation();
    };
  });

  // Ẩn dropdown khi click ngoài
  document.addEventListener('click', function () {
    modal.querySelectorAll('.custom-dropdown-menu').forEach(m => m.classList.add('d-none'));
  });

  // Sửa comment
  modal.querySelectorAll('.btn-edit-comment').forEach(btn => {
    btn.onclick = function () {
      const commentId = this.dataset.commentId;
      modal.querySelector(`#text-${commentId}`).style.display = 'none';
      modal.querySelector(`#comment-content-${commentId} .edit-comment-form`).classList.remove('d-none');
    };
  });

  // Huỷ sửa
  modal.querySelectorAll('.cancel-edit').forEach(btn => {
    btn.onclick = function () {
      const form = this.closest('.edit-comment-form');
      const commentId = form.dataset.id;
      form.classList.add('d-none');
      modal.querySelector(`#text-${commentId}`).style.display = 'block';
    };
  });

  // Gửi sửa
  modal.querySelectorAll('.edit-comment-form').forEach(form => {
    form.onsubmit = async function (e) {
      e.preventDefault();
      const commentId = this.querySelector('input[name="comment_id"]').value;
      const postId = this.querySelector('input[name="post_id"]').value;
      const content = this.querySelector('textarea[name="comment_content"]').value;

      const res = await fetch('/Post/EditComment', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ comment_id: commentId, post_id: postId, comment_content: content })
      });

      if (res.ok) {
        modal.querySelector(`#text-${commentId}`).textContent = content;
        modal.querySelector(`#text-${commentId}`).style.display = 'block';
        this.classList.add('d-none');
        reloadDetailComment("Cập nhật bình luận thành công");
      }
    };
  });
}

//Open again popup delete in detail popup
function bindDynamicModalInDetail() {
  const modal = document.querySelector('#detailPostModal');
  if (!modal) return;

  modal.querySelectorAll(".open-dynamic-modal").forEach(btn => {
    btn.addEventListener("click", function () {
      const title = btn.dataset.title || "Xác nhận";
      const action = btn.dataset.action;
      const controller = btn.dataset.controller;
      const method = btn.dataset.method || "POST";
      const type = btn.dataset.type || "";
      const id = btn.dataset.id;

      const dynamicForm = document.getElementById("dynamicForm");
      const formFieldsContainer = document.getElementById("formFields");

      // Cập nhật cấu hình form
      dynamicForm.setAttribute("data-action", action);
      dynamicForm.setAttribute("data-controller", controller);
      dynamicForm.setAttribute("data-method", method);
      dynamicForm.setAttribute("data-type", type);

      // Gán callback riêng nếu đang ở detail popup
      dynamicForm.setAttribute("data-success-callback", "reloadDetailComment");

      document.getElementById("dynamicModalLabel").textContent = title;
      formFieldsContainer.innerHTML = `
        <input type="hidden" name="id" value="${id}" />
        <p>Bạn có chắc chắn muốn xoá mục này không?</p>
      `;

      // Show modal
      const confirmModal = new bootstrap.Modal(document.getElementById("dynamicModal"));
      confirmModal.show();
    });
  });
}

//Hiển thị danh sách người dùng đã tương tác bài post
function showReactionsPopup(postId) {
  console.log("📌 Calling GetReactions with postId =", postId);

  fetch(`/Post/GetReactions?postId=${postId}`)
    .then(res => res.text())
    .then(html => {
      // Thiết lập lại modal dùng chung
      const form = document.getElementById("dynamicForm");
      form.setAttribute("data-type", "reaction");
      form.setAttribute("data-method", "GET");
      form.setAttribute("data-action", ""); // Không cần action nếu chỉ để hiển thị
      form.setAttribute("data-controller", ""); // Không cần

      // Set title
      document.getElementById("dynamicModalLabel").textContent = "Danh sách người đã tương tác";

      // Hiển thị nội dung trong vùng form
      document.getElementById("formFields").innerHTML = html;

      // Ẩn nút Submit vì chỉ xem
      document.getElementById("dynamicSubmitButton").style.display = "none";

      // Show popup
      const modal = new bootstrap.Modal(document.getElementById("dynamicModal"));
      modal.show();
    })
    .catch(err => {
      document.getElementById("formFields").innerHTML =
        "<div class='text-danger'>Không thể tải danh sách.</div>";
      const modal = new bootstrap.Modal(document.getElementById("dynamicModal"));
      modal.show();
      console.error("❌ Lỗi khi tải danh sách người tương tác:", err);
    });
}


function reloadDetailComment(customMessage = null, postId = null) { // 👈 Adjust param nếu cần postId từ handler
  // Nếu không có postId từ param, lấy từ input trong modal
  const postIdInput = document.querySelector("#detailPostModal input[name='post_id']");
  const postIdFinal = postId || (postIdInput ? postIdInput.value : null);
  
  if (!postIdFinal) {
    console.error("❌ Không tìm thấy postId để reload comment.");
    hideGlobalLoader(); // Đảm bảo hide nếu lỗi
    return;
  }

  fetch(`/Post/Details?postId=${postIdFinal}`)
    .then(r => {
      if (!r.ok) throw new Error("Lỗi fetch details.");
      return r.text();
    })
    .then(html => {
      const parser = new DOMParser();
      const newDoc = parser.parseFromString(html, "text/html");
      const newCommentArea = newDoc.querySelector(".overflow-auto");
      const modalCommentArea = document.querySelector("#detailPostModal .overflow-auto");

      if (newCommentArea && modalCommentArea) {
        modalCommentArea.innerHTML = newCommentArea.innerHTML;

        rebindEditDeleteHandlersInDetail();
        bindDynamicModalInDetail();
      }

      const modalEl = document.getElementById("dynamicModal");
      const bsModal = bootstrap.Modal.getInstance(modalEl);
      if (bsModal) bsModal.hide();

      // 👈 THÊM MỚI: Show notification và HIDE LOADER
      const message = customMessage || "Xóa bình luận thành công.";
      showNotification(message, "success");
      hideGlobalLoader(); // 👈 QUAN TRỌNG: Ẩn loader sau khi reload done
    })
    .catch(error => {
      console.error("❌ Lỗi reload comment:", error);
      showNotification("Không thể cập nhật danh sách bình luận.", "danger");
      hideGlobalLoader(); // 👈 Luôn hide ở error
    });
}

function bindEmojiPicker(scope = document) {
  scope.querySelectorAll('form').forEach(form => {
    if (form.dataset.emojiBound) return; // ❗ chỉ bind 1 lần
    form.dataset.emojiBound = "true";
    const emojiContainer = form.parentElement.querySelector('.emoji-picker');
    const emojiToggleBtn = form.querySelector('.emoji-toggle-btn');
    const commentInput = form.querySelector('.comment-input') || form.querySelector('textarea');

    if (!emojiContainer || !emojiToggleBtn || !commentInput) return;

    // Toggle emoji picker hiển thị
    emojiToggleBtn.addEventListener('click', (e) => {
      e.stopPropagation(); // Ngăn click lan ra ngoài
      emojiContainer.classList.toggle('d-none');
    });

    // Nếu chưa load emoji thì load từ API
    // if (!emojiContainer.dataset.loaded) {
    //   fetch('https://emoji-api.com/emojis?access_key=fa7355d4de4d35ce94606110ea69b9e0d81041cf')
    //     .then(res => res.json())
    //     .then(data => {
    //       data.slice(0, 40).forEach(emoji => {
    //         const button = document.createElement('button');
    //         button.classList.add('emoji-btn');
    //         button.type = 'button';
    //         button.textContent = emoji.character;
    //         button.addEventListener('click', () => {
    //           commentInput.value += emoji.character;
    //           commentInput.focus();
    //           emojiContainer.classList.add('d-none'); // ⛔ Ẩn luôn khi chọn emoji (tuỳ ý)
    //         });
    //         emojiContainer.appendChild(button);
    //       });
    //       emojiContainer.dataset.loaded = "true";
    //     })
    //     .catch(error => {
    //       console.error('Lỗi khi load emoji:', error);
    //     });
    // }

    if (!emojiContainer.dataset.loaded) {
      const emojiList = [
        "😀", "😃", "😄", "😁", "😆", "😅", "😂", "🤣", "😊", "😇",
        "🙂", "🙃", "😉", "😍", "😘", "😋", "😎", "😤", "😭", "😡",
        "👍", "👎", "👏", "🙏", "💖", "🔥", "💯", "🎉", "🥳", "🤯",
        "😴", "😷", "🤒", "🤧", "🥶", "🥵", "😈", "👻", "💀", "🫶"
      ];

      emojiList.forEach(character => {
        const button = document.createElement('button');
        button.classList.add('emoji-btn');
        button.type = 'button';
        button.textContent = character;
        button.addEventListener('click', () => {
          commentInput.value += character;
          commentInput.focus();
          emojiContainer.classList.add('d-none');
        });
        emojiContainer.appendChild(button);
      });

      emojiContainer.dataset.loaded = "true";
    }
    // Ẩn emoji nếu click bên ngoài
    document.addEventListener('click', (e) => {
      if (
        !emojiToggleBtn.contains(e.target) &&
        !emojiContainer.contains(e.target)
      ) {
        emojiContainer.classList.add('d-none');
      }
    });

    // ⌨️ Ẩn emoji khi gõ nội dung
    commentInput.addEventListener('input', () => {
      emojiContainer.classList.add('d-none');
    });
  });
}

