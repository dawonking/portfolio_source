#include "SharedObject1.h"
#include "opencv2/core.hpp"
#include "opencv2/opencv.hpp"
#include "opencv2/features2d.hpp"
#include "opencv2/imgcodecs.hpp"
#include "opencv2/core/ocl.hpp"
#include <iostream>
#include <android/log.h>


#define LOGI(...) ((void)__android_log_print(ANDROID_LOG_INFO, "SharedObject1", __VA_ARGS__))
#define LOGW(...) ((void)__android_log_print(ANDROID_LOG_WARN, "SharedObject1", __VA_ARGS__))
using namespace cv;
using namespace std;
using namespace ocl;

double check_value_output(vector<Point>& grid_point, vector<double>& value, Mat& input, Mat& Background);

double back_angle(Mat& input, Mat& Background);

double getangle(const Point& p1, const Point& p2);

Mat _currentFrame;
string path;
Mat H;

Mat m_copy;

Mat cut_list[6] = {};

Mat cut_list_1;
Mat cut_list_2;
Mat cut_list_3;
Mat cut_list_4;
Mat cut_list_5;
Mat cut_list_6;

Mat res, res_norm;

clock_t pass_start = 0;
clock_t pass_end = 0;
Rect crop_rect((640 / 2 - 224 / 2), (360 / 2 - 224 / 2), 224, 224);

vector<Point> maxloc_list;
vector<double> match_point;

vector<Mat> insert_Mat;

double maxv;
Point maxloc;
Point center_point;
double set_an = 0;

extern "C"
{

    float SharedObject1::Foopluginmethod()
    {
        return 1;     // should return 10.0f
    }


    struct Color32
    {
        uchar red;
        uchar green;
        uchar blue;
        uchar alpha;
    };

	//test_image_pass
    void SharedObject1::ProcessImage(Color32** rawImage1, int width1, int height1)
    {
        Mat Image(height1, width1, CV_8UC4, *rawImage1);

        Mat cp = Image.clone();
        cvtColor(cp, cp, COLOR_RGB2GRAY);

        Image = cp.clone();

        //imwrite("", Image);

        //Readimg(height1, width1, CV_8UC4,  *rawImage1);      

        flip(Image, Image, -1);
    }


	//save_capture_image android local folder
    bool SharedObject1::saveMat(Color32** rawImage1, int width1, int height1, char* str)
    {
        try {
            Mat Image(height1, width1, CV_8UC4, *rawImage1);

            //colal(height1, width1, CV_8UC4, *rawImage1);
            cvtColor(Image, Image, COLOR_RGB2BGR);
            cvtColor(Image, Image, COLOR_BGR2GRAY);
            vector<int> pa;
            pa.push_back(IMWRITE_JPEG_QUALITY);
            pa.push_back(95);
            //imwrite(str, Image,pa);

            cvSaveImage(str, &Image);

            Image.release();
            return true;
        }
        catch (Exception e) {

            return false;

        }
    }


	//original image , output_image
	//return 
    double SharedObject1::check_value(unsigned char* input_data, unsigned char* output_data, int width, int height)
    {
        try {
            clock_t start, end;
            start = clock();

            maxloc_list.clear();
            match_point.clear();

            Mat input_tex(width, height, CV_8UC4, input_data);
            Mat in_copy(input_tex);
            Mat out_copy(240, 240, CV_8UC4, output_data);            

            cvtColor(in_copy, in_copy, COLOR_RGB2BGR);
            cvtColor(out_copy, out_copy, COLOR_RGBA2BGRA);

            if (in_copy.total() < 1) {
                return -11;
            }
           
            try {

                resize(in_copy, in_copy, Size(240, 240), 1, 1, INTER_LINEAR);
                double ch = 0;

                for (int i = 0; i < 6; i++) {
                    check_value_output(maxloc_list, match_point, cut_list[i], in_copy);
                }

                for (int i = 0; i < 6; i++) {

                    if (match_point[i] > 0.98) {
                        rectangle(in_copy, Rect(maxloc_list[i].x, maxloc_list[i].y, cut_list[i].cols, cut_list[i].rows), Scalar::all(-1), 1);
                    }
                }

				//return color
                cvtColor(in_copy, out_copy, COLOR_BGR2RGBA);


                memcpy(output_data, out_copy.data, out_copy.total() * out_copy.elemSize());

                input_tex.release();

                in_copy.release();
                out_copy.release();

                end = clock();

                return  (double)(end - start) / CLOCKS_PER_SEC;

            }
            catch (Exception e) {
                return -12;
            }

        }
        catch (Exception e) {
            return -1;
        }



    }


	//image_load
    bool SharedObject1::foo(char* str)
    {
        try {

            path = str;
            //_currentFrame = imread(str);
            //_currentFrame.size();   

            //resize(_currentFrame, _currentFrame, Size(640, 270), 2, 1, INTER_AREA);

            _currentFrame = imread(path + "/save.png", IMREAD_COLOR);

            cut_list_1 = imread(path + "/save_cut_6.jpg", IMREAD_COLOR);
            cut_list_2 = imread(path + "/save_cut_7.jpg", IMREAD_COLOR);
            cut_list_3 = imread(path + "/save_cut_8.jpg", IMREAD_COLOR);
            cut_list_4 = imread(path + "/save_cut_9.jpg", IMREAD_COLOR);
            cut_list_5 = imread(path + "/save_cut_10.jpg", IMREAD_COLOR);
            cut_list_6 = imread(path + "/save_cut_11.jpg", IMREAD_COLOR);

            flip(cut_list_1, cut_list_1, 0);
            flip(cut_list_2, cut_list_2, 0);
            flip(cut_list_3, cut_list_3, 0);
            flip(cut_list_4, cut_list_4, 0);
            flip(cut_list_5, cut_list_5, 0);
            flip(cut_list_6, cut_list_6, 0);

            flip(_currentFrame, _currentFrame, 0);




            cut_list[0] = cut_list_1;
            cut_list[1] = cut_list_2;
            cut_list[2] = cut_list_3;
            cut_list[3] = cut_list_4;
            cut_list[4] = cut_list_5;
            cut_list[5] = cut_list_6;

            if (size(cut_list) > 0) {
                return true;
            }
            else {
                return false;
            }

        }
        catch (Exception e) {
            return false;
        }
    }

	// input_image and check save images
	// use opencv image_matching, and return value
    bool SharedObject1::matching_input(unsigned char* input_data, int width, int height, double c_point[], double mx[], double my[], double set_angle)
    {
        try {
            Mat input_tex(width, height, CV_8UC4, input_data);
            Mat in_copy(input_tex);



            cvtColor(in_copy, in_copy, COLOR_RGB2BGR);

            if (in_copy.total() < 1) {
                return false;
            }

            try {

				// match six image and input array value
                for (int i = 0; i < 6; i++) {
                    check_value_output(maxloc_list, match_point, cut_list[i], in_copy);
                }

                for (int i = 0; i < match_point.size(); i++) {
                    c_point[i] = match_point[i];
                    mx[i] = maxloc_list[i].x;
                    my[i] = maxloc_list[i].y;
                }


                maxloc_list.clear();
                match_point.clear();
                input_tex.release();
                in_copy.release();

                return true;
            }
            catch (Exception e) {
                return false;
            }

        }
        catch (Exception e) {
            return false;
        }
    }

    int SharedObject1::input_vec_mat(unsigned char* input_data, int width, int height)
    {

        Mat input_tex(width, height, CV_8UC4, input_data);

        insert_Mat.push_back(input_tex);

        input_tex.release();

        return insert_Mat.size();
    }

   
}


// input -> cut_list , Background -> video_image, gray
double check_value_output(vector<Point>& grid_point, vector<double>& value, Mat& input, Mat& Background) {
    /*Mat res = Mat::zeros(0, 0, CV_32FC1);
    Mat res_norm = Mat::zeros(0, 0, CV_8UC1);
 */

 //Mat res, res_norm;

    matchTemplate(Background, input, res, TM_CCOEFF_NORMED);
    normalize(res, res_norm, 0, 255, NORM_MINMAX, CV_8U);
    minMaxLoc(res, 0, &maxv, 0, &maxloc);

    center_point.x = (maxloc.x + input.cols) / 2;
    center_point.y = (maxloc.y + input.rows) / 2;

    if (maxv >= 1) {
        maxv = 0;
    }

    grid_point.push_back(center_point);
    value.push_back(maxv);

    res.release();
    res_norm.release();

    return maxv;
}

double back_angle(Mat& input, Mat& Background) {

    Mat res, res_norm;

    matchTemplate(Background, input, res, TM_CCOEFF_NORMED);
    normalize(res, res_norm, 0, 255, NORM_MINMAX, CV_8U);
    minMaxLoc(res, 0, &maxv, 0, &maxloc);
    res.release();
    res_norm.release();

    set_an = getangle(maxloc, Point(Background.cols / 2, Background.rows));

    return set_an;
}


double getangle(const Point& p1, const Point& p2) {

    double xdf = p2.x - p1.x;
    double ydf = p2.y - p1.y;

    double radian = atan2(ydf, xdf);
    double degree = radian * 57.3f;

    return degree;
}