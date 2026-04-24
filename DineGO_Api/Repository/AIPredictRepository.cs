using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Models;
using DineGO_Api.Data;

namespace DineGO_Api.Repository
{
    public class AIPredictRepository : IAIPredictRepository
    {
        private readonly PriorityDAO _priorityDAO;
        private readonly CategoryDAO _categoryDAO;
        private readonly FoodDAO _foodDAO;

        public AIPredictRepository(PriorityDAO priorityDAO, CategoryDAO categoryDAO, FoodDAO foodDAO)
        {
            _priorityDAO = priorityDAO;
            _categoryDAO = categoryDAO;
            _foodDAO = foodDAO;
        }
        public async Task<int> UpdateTagsToCategoryAsync()
        {
            var basePath = Directory.GetCurrentDirectory();
            var labelPath = Path.Combine(basePath, "AIPredict", "models", "intent-food-model", "labels.json");

            if (!File.Exists(labelPath))
                throw new FileNotFoundException("Không tìm thấy labels.json tại: " + labelPath);

            var json = await File.ReadAllTextAsync(labelPath);
            var label2id = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
            if (label2id == null || label2id.Count == 0)
                return 0;

            int added = 0;

            foreach (var tag in label2id.Keys)
            {
                if (!_categoryDAO.ExistsByType(tag))
                {
                    _categoryDAO.SaveCategory(new Category
                    {
                        cate_type = tag,
                        cate_description = "AI Tag: " + tag
                    });
                    added++;
                }
            }

            return added;
        }
        public async Task<List<string>> SuggestValidTagsAsync(string text)
        {
            var basePath = Directory.GetCurrentDirectory();
            var pyPath = Path.Combine(basePath, "AIPredict", "venv", "Scripts", "python.exe");
            var scriptPath = Path.Combine(basePath, "AIPredict", "predict_single.py");

            if (!File.Exists(pyPath)) throw new FileNotFoundException("Không tìm thấy Python", pyPath);
            if (!File.Exists(scriptPath)) throw new FileNotFoundException("Không tìm thấy script", scriptPath);

            var psi = new ProcessStartInfo
            {
                FileName = pyPath,
                Arguments = $"\"{scriptPath}\" \"{text}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };


            using var process = Process.Start(psi);
            string output = await process.StandardOutput.ReadToEndAsync();
            string errors = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(errors))
                throw new Exception("Python error: " + errors);

            var predictedTags = JsonSerializer.Deserialize<List<string>>(output) ?? new List<string>();

            // Lọc tag theo Category
            var validTags = predictedTags
                .Where(tag => _categoryDAO.ExistsByType(tag))
                .ToList();

            return validTags;
        }
        public async Task<int> UpdatePriorityFromTextAsync(int cusId, string text)
        {
            var basePath = Directory.GetCurrentDirectory();
            var pyPath = Path.Combine(basePath, "AIPredict", "venv", "Scripts", "python.exe");
            var scriptPath = Path.Combine(basePath, "AIPredict", "predict_single.py");

            if (!File.Exists(pyPath)) throw new FileNotFoundException("Không tìm thấy Python", pyPath);
            if (!File.Exists(scriptPath)) throw new FileNotFoundException("Không tìm thấy script", scriptPath);

            var psi = new ProcessStartInfo
            {
                FileName = pyPath,
                Arguments = $"\"{scriptPath}\" \"{text}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            string output = await process.StandardOutput.ReadToEndAsync();
            string errors = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(errors))
                throw new Exception("Python error: " + errors);

            var tags = JsonSerializer.Deserialize<List<string>>(output) ?? new();

            int updated = 0;

            foreach (var tag in tags)
            {
                if (_categoryDAO.ExistsByType(tag))
                {
                    _priorityDAO.AddOrIncrement(tag, cusId);
                    updated++;
                }
            }

            return updated;
        }

        public void AddClickToTag(string tag, int cusId)
        {
            _priorityDAO.AddClick(tag, cusId);
        }

        public void SetManualPriorityWeight(string tag, int cusId, double weight)
        {
            _priorityDAO.SetManualWeight(tag, cusId, weight);
        }

        public async Task<string> GenerateFoodSuggestionAsync(string userInput)
        {
            // Đảm bảo đây là path đến AIPredict (nơi có venv, models, script)
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "AIPredict");
            var pyPath = Path.Combine(basePath, "venv", "Scripts", "python.exe");
            var scriptPath = Path.Combine(basePath, "predict_response.py");

            if (!File.Exists(pyPath)) throw new FileNotFoundException("Không tìm thấy Python", pyPath);
            if (!File.Exists(scriptPath)) throw new FileNotFoundException("Không tìm thấy script", scriptPath);

            var psi = new ProcessStartInfo
            {
                FileName = pyPath,
                Arguments = $"\"{scriptPath}\" \"{userInput}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = basePath // Key point: phải đặt working dir về đúng nơi!
            };

            using var process = Process.Start(psi);
            string output = await process.StandardOutput.ReadToEndAsync();
            string errors = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(errors))
                throw new Exception("Python error: " + errors);

            return output.Trim();
        }
        public async Task<object> GetSuggestionWithFoodsAsync(string userInput)
        {
            // B1: Gọi model response (gợi ý từ AI)
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "AIPredict");
            var pyResponsePath = Path.Combine(basePath, "venv", "Scripts", "python.exe");
            var scriptResponse = Path.Combine(basePath, "predict_response.py");

            if (!File.Exists(pyResponsePath)) throw new FileNotFoundException("Không tìm thấy Python", pyResponsePath);
            if (!File.Exists(scriptResponse)) throw new FileNotFoundException("Không tìm thấy script", scriptResponse);

            var psiResponse = new ProcessStartInfo
            {
                FileName = pyResponsePath,
                Arguments = $"\"{scriptResponse}\" \"{userInput}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = basePath
            };

            using var processResponse = Process.Start(psiResponse);
            string fullOutput = await processResponse.StandardOutput.ReadToEndAsync();
            string errorResponse = await processResponse.StandardError.ReadToEndAsync();
            processResponse.WaitForExit();

            if (!string.IsNullOrWhiteSpace(errorResponse))
                throw new Exception("Python response error: " + errorResponse);

            // B2: Tách <br>
            var parts = fullOutput.Split("<br>");
            var response = parts[0].Trim();
            var context = parts.Length > 1 ? parts[1].Trim() : "";

            // B3: Dự đoán tag từ context
            var scriptPredict = Path.Combine(basePath, "predict_single.py");
            var psiTag = new ProcessStartInfo
            {
                FileName = pyResponsePath,
                Arguments = $"\"{scriptPredict}\" \"{context}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var processTag = Process.Start(psiTag);
            string tagOutput = await processTag.StandardOutput.ReadToEndAsync();
            string tagError = await processTag.StandardError.ReadToEndAsync();
            processTag.WaitForExit();

            if (!string.IsNullOrWhiteSpace(tagError))
                throw new Exception("Python tag error: " + tagError);

            var predictedTags = JsonSerializer.Deserialize<List<string>>(tagOutput) ?? new();
            var validTags = predictedTags.Where(tag => _categoryDAO.ExistsByType(tag)).ToList();

            // B4: Truy vấn món ăn
            var foods = _foodDAO.GetFoodsByTags(validTags);

            return new
            {
                response,
                tags = validTags,
                foods
            };
        }
    }
}