using System;
using System.IO;
using System.Collections.Generic;
class Program {
    static void Main(String[] args){
        Console.WriteLine("args are: "+args[0]);
        Program p = new Program();
        p.getFileSize(args[0]);
        p.getDirs(args[0]);
        Dictionary<string, long> finalAns = p.calcSize(args[0]);
        foreach (KeyValuePair<string, long> kvp in finalAns)
        {
            Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
        }
        
    }

    Dictionary<string, long> calcSize(String dir){
        // Recurse into folders and calculate sizes
        string[] MainDirs = getDirs(dir);        
        Dictionary<string, long> finalDirSize = new Dictionary<string, long>();
        if(MainDirs.Length == 0){
            // Folder has no subDirs, calc size
            Dictionary<string, long> totalDirSize = getFileSize(dir);
            Dictionary<string, long> innerDirSize = new Dictionary<string, long>();
            long sum = 0;
            foreach (KeyValuePair<string, long> kvp in totalDirSize)
            {
                if(kvp.Key != dir)
                sum += kvp.Value;
            }
            finalDirSize.Add(dir, sum);
            return finalDirSize;
            
        }
        else{
            Dictionary<string, long> fileDict = getFileSize(dir);
            long fileSum = 0;
            foreach (KeyValuePair<string, long> kvp in fileDict){
                if(kvp.Key != dir)
                fileSum+=kvp.Value;
            }
            long dirSum = 0;
            foreach (string name in MainDirs){
                // Call CalcSize to find inner subDirs
                Dictionary<string, long> ans = calcSize(name);
                dirSum += ans[name];
                finalDirSize.Add(name, ans[name]);
            }
            finalDirSize.Add(dir, dirSum+fileSum);
            finalDirSize.Add("files", fileSum);
            return finalDirSize;
        }
        
        
    }

    string[] getDirs(String dir){
        // Console.WriteLine("All dirs are: ");
        string[] dirs = Directory.GetDirectories(dir);
        // foreach (string name in dirs){
        //     Console.WriteLine(name);
        // }
        return dirs;
    }
    
    Dictionary<string, long> getFileSize(String dir){
        Dictionary<string, long> files = new Dictionary<string, long>();
        string[] filesArr = Directory.GetFiles(dir);
        long totalSize = 0;
        foreach (string name in filesArr){
            FileInfo info = new FileInfo(name);
            long size = info.Length;
            totalSize+=size;
            files.Add(name, size);
        }
        files.Add(dir, totalSize);
        return files;
    }
}
