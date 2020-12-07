﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
class Program {
    static void Main(String[] args){
        Console.WriteLine("Specified Directory is: "+args[0]);
        Program p = new Program();
        p.getFileSize(args[0]);
        p.getDirs(args[0]);
        Dictionary<string, decimal> finalAns = p.calcSize(args[0]);
        var sortedSize = finalAns.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        foreach (KeyValuePair<string, decimal> kvp in sortedSize)
        {
            if((kvp.Value / 1024 /1024) < 1){
                // finalAns[kvp.Key] = kvp.Value / 1024;
                Console.WriteLine("Dir = {0}, Size = {1}", kvp.Key, (kvp.Value/1024).ToString("#.##") +" KB");
            }
            else if((kvp.Value / 1024 / 1024 / 1024) < 1){
                // finalAns[kvp.Key] = kvp.Value/1024;
                Console.WriteLine("Dir = {0}, Size = {1}", kvp.Key, (kvp.Value/1024/1024).ToString("#.##") +" MB");
            }
            else{
                Console.WriteLine("Dir = {0}, Size = {1}", kvp.Key, (kvp.Value/1024/1024/1024).ToString("#.##") +" GB");
            }
            
        }
        
    }

    Dictionary<string, decimal> calcSize(String dir){
        // Recurse into folders and calculate sizes
        string[] MainDirs = getDirs(dir);        
        Dictionary<string, decimal> finalDirSize = new Dictionary<string, decimal>();
        if(MainDirs.Length == 0){
            // Folder has no subDirs, calc size
            Dictionary<string, decimal> totalDirSize = getFileSize(dir);
            Dictionary<string, decimal> innerDirSize = new Dictionary<string, decimal>();
            decimal sum = 0;
            foreach (KeyValuePair<string, decimal> kvp in totalDirSize)
            {
                if(kvp.Key != dir)
                sum += kvp.Value;
            }
            finalDirSize.Add(dir, sum);
            return finalDirSize;
            
        }
        else{
            Dictionary<string, decimal> fileDict = getFileSize(dir);
            decimal fileSum = 0;
            foreach (KeyValuePair<string, decimal> kvp in fileDict){
                if(kvp.Key != dir)
                fileSum+=kvp.Value;
            }
            decimal dirSum = 0;
            foreach (string name in MainDirs){
                // Call CalcSize to find inner subDirs
                Dictionary<string, decimal> ans = calcSize(name);
                dirSum += ans[name];
                finalDirSize.Add(name, ans[name]);
            }
            finalDirSize.Add(dir, dirSum+fileSum);
            finalDirSize.Add("files", fileSum);
            return finalDirSize;
        }
        
        
    }

    string[] getDirs(String dir){
        string[] dirs = Directory.GetDirectories(dir);
        return dirs;
    }
    
    Dictionary<string, decimal> getFileSize(String dir){
        Dictionary<string, decimal> files = new Dictionary<string, decimal>();
        string[] filesArr = Directory.GetFiles(dir);
        decimal totalSize = 0;
        foreach (string name in filesArr){
            FileInfo info = new FileInfo(name);
            decimal size = info.Length;
            totalSize+=size;
            files.Add(name, size);
        }
        files.Add(dir, totalSize);
        return files;
    }
}
